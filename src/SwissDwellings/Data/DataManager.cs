using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwissDwellings.Data
{
    public static class DataManager
    {
        private const string DatasetUrl = "https://zenodo.org/records/7788422/files/swiss-dwellings-v3.0.0.zip?download=1";

        public static string GetPath()
        {
            // Default path for the dataset zip
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SwissDwellings", "dataset.zip");
        }

        public static string GetExtractedPath()
        {
            // Path where data is extracted
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SwissDwellings", "data");
        }

        public static async Task EnsureDataAsync(Action<string>? log = null)
        {
            var safeLog = log ?? Console.WriteLine;
            var zipPath = GetPath();
            var extractPath = GetExtractedPath();

            if (!File.Exists(zipPath) && !Directory.Exists(extractPath))
            {
                safeLog($"Downloading SwissDwellings dataset to {zipPath}...");

                // Ensure directory exists
                var dir = Path.GetDirectoryName(zipPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Download logic
                using (var client = new HttpClient())
                {
                    // Use a stream to download to avoid loading large file into memory
                    using (var response = await client.GetAsync(DatasetUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                safeLog("Download complete.");
            }

            if (File.Exists(zipPath) && !Directory.Exists(extractPath))
            {
                safeLog($"Extracting dataset to {extractPath}...");
                try
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                }
                catch (Exception ex)
                {
                     // Cleanup on failure: delete the potentially corrupted zip so it can be re-downloaded
                     if (File.Exists(zipPath))
                     {
                         try { File.Delete(zipPath); } catch { }
                     }
                     if (Directory.Exists(extractPath))
                     {
                         try { Directory.Delete(extractPath, true); } catch { }
                     }

                     throw new InvalidOperationException($"Failed to extract dataset: {ex.Message}. Corrupted zip file has been deleted.", ex);
                }
                safeLog("Extraction complete.");
            }

            // Verify
            if (!Directory.Exists(extractPath))
            {
                 // If zip exists but extract failed or didn't happen
                 if (!File.Exists(zipPath))
                 {
                    throw new FileNotFoundException($"Dataset failed to download or does not exist at {zipPath}.");
                 }
                 // If extraction was skipped for some reason
                 throw new DirectoryNotFoundException($"Dataset extraction failed or directory does not exist at {extractPath}.");
            }
        }
    }
}
