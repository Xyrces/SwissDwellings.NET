using System;
using System.IO;
using System.Threading.Tasks;

namespace SwissDwellings.Data
{
    public static class DataManager
    {
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

                // Download logic here (Placeholder)
                // For now, we won't actually download anything to avoid external calls,
                // but this is where the download and extraction would happen.
                await Task.CompletedTask;

                // Verify file exists after "download"
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Dataset failed to download or does not exist at {path}. Download logic is currently a placeholder.");
                }
            }
        }
    }
}
