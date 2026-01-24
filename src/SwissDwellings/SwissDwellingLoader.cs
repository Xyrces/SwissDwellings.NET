using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SwissDwellings.Data;

namespace SwissDwellings
{
    public static class SwissDwellingLoader
    {
        public static async Task<List<SwissDwellingLayout>> LoadLayoutsAsync(
            string? path = null,
            string? scriptPath = null,
            string pythonExecutable = "python",
            Action<string>? logger = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                await DataManager.EnsureDataAsync(logger ?? (_ => { }));
                path = DataManager.GetPath();
            }

            if (string.IsNullOrEmpty(scriptPath))
            {
                throw new ArgumentException("scriptPath must be provided", nameof(scriptPath));
            }

            var psi = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            psi.ArgumentList.Add(scriptPath);
            psi.ArgumentList.Add(path);

            using (var p = new Process { StartInfo = psi })
            {
                p.Start();

                var jsonTask = p.StandardOutput.ReadToEndAsync();
                var errorTask = p.StandardError.ReadToEndAsync();

                await p.WaitForExitAsync();

                if (p.ExitCode != 0)
                {
                    var error = await errorTask;
                    throw new InvalidOperationException($"Python script failed with exit code {p.ExitCode}. Error: {error}");
                }

                var json = await jsonTask;

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<SwissDwellingLayout>();
                }

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<SwissDwellingLayout>>(json, options)
                           ?? new List<SwissDwellingLayout>();
                }
                catch (JsonException ex)
                {
                     throw new InvalidOperationException($"Failed to deserialize Python output: {json}", ex);
                }
            }
        }
    }
}
