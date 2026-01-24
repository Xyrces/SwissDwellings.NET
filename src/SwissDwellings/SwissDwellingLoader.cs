using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SwissDwellings.Data;

namespace SwissDwellings
{
    public static class SwissDwellingLoader
    {
        private static string? _defaultPythonExecutable;

        private static string GetDefaultPythonExecutable()
        {
            if (_defaultPythonExecutable != null) return _defaultPythonExecutable;

            // Try python3 first, then python
            try
            {
                var p = Process.Start(new ProcessStartInfo("python3", "--version") { CreateNoWindow = true, UseShellExecute = false });
                p?.WaitForExit();
                if (p?.ExitCode == 0)
                {
                    return _defaultPythonExecutable = "python3";
                }
            }
            catch
            {
                // Ignore and try python
            }

            return _defaultPythonExecutable = "python";
        }

        public static async Task<List<SwissDwellingLayout>> LoadLayoutsAsync(
            string? path = null,
            string? pythonExecutable = null,
            Action<string>? logger = null)
        {
            // Resolve the internal script path
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var internalScriptPath = Path.Combine(assemblyLocation!, "Resources", "loader.py");

            return await LoadLayoutsInternalAsync(path, internalScriptPath, pythonExecutable, logger);
        }

        internal static async Task<List<SwissDwellingLayout>> LoadLayoutsInternalAsync(
            string? path = null,
            string? scriptPath = null,
            string? pythonExecutable = null,
            Action<string>? logger = null)
        {
            // Default logger to Console.WriteLine if not provided
            var effectiveLogger = logger ?? Console.WriteLine;
            var effectivePythonExecutable = pythonExecutable ?? GetDefaultPythonExecutable();

            if (string.IsNullOrEmpty(path))
            {
                await DataManager.EnsureDataAsync(effectiveLogger);
                path = DataManager.GetExtractedPath();
            }

            // Ensure we are pointing to the extracted folder if a zip was passed but not handled?
            // Actually DataManager.GetExtractedPath() should handle returning the folder.

            if (string.IsNullOrEmpty(scriptPath))
            {
                 throw new ArgumentException("scriptPath must be provided", nameof(scriptPath));
            }

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Python loader script not found at {scriptPath}");
            }

            var psi = new ProcessStartInfo
            {
                FileName = effectivePythonExecutable,
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
