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
            // Default path for the dataset
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SwissDwellings", "dataset.zip");
        }

        public static async Task EnsureDataAsync(Action<string> log)
        {
            var path = GetPath();
            if (!File.Exists(path))
            {
                log($"Downloading SwissDwellings dataset to {path}...");

                // Ensure directory exists
                var dir = Path.GetDirectoryName(path);
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
                        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                log("Download complete.");

                // Note: Extraction logic would go here if we were processing the zip,
                // but the requirements imply passing the path (likely the zip or a folder) to the loader.
                // Assuming the python script handles the zip or we just need the file.
                // If extraction is needed:
                /*
                log("Extracting dataset...");
                ZipFile.ExtractToDirectory(path, dir);
                */

                // Verify file exists after download
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Dataset failed to download or does not exist at {path}.");
                }
            }
        }
    }
}
