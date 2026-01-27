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
        internal static string? ScriptOverride { get; set; }

        private static async Task<string> GetDefaultPythonExecutableAsync()
        {
            if (_defaultPythonExecutable != null) return _defaultPythonExecutable;

            // Try python3 first
            try
            {
                var p = Process.Start(new ProcessStartInfo("python3", "--version") { CreateNoWindow = true, UseShellExecute = false });
                if (p != null)
                {
                    await p.WaitForExitAsync();
                    if (p.ExitCode == 0)
                    {
                        return _defaultPythonExecutable = "python3";
                    }
                }
            }
            catch
            {
                // Ignore
            }

            // Try python
            try
            {
                var p = Process.Start(new ProcessStartInfo("python", "--version") { CreateNoWindow = true, UseShellExecute = false });
                if (p != null)
                {
                    await p.WaitForExitAsync();
                    if (p.ExitCode == 0)
                    {
                        return _defaultPythonExecutable = "python";
                    }
                }
            }
            catch
            {
                // Ignore
            }

            throw new InvalidOperationException("Could not find 'python3' or 'python' on the system PATH. Please install Python to use this library.");
        }

        public static async Task<List<SwissDwellingLayout>> LoadLayoutsAsync(
            string? path = null,
            string? pythonExecutable = null,
            Action<string>? logger = null)
        {
            // Default logger to Console.WriteLine if not provided
            var effectiveLogger = logger ?? Console.WriteLine;
            var effectivePythonExecutable = pythonExecutable ?? await GetDefaultPythonExecutableAsync();

            if (string.IsNullOrEmpty(path))
            {
                await DataManager.EnsureDataAsync(effectiveLogger);
                path = DataManager.GetExtractedPath();
            }

            // Resolve script path (Internal override for testing or Bundled resource)
            var scriptPath = ScriptOverride;
            if (string.IsNullOrEmpty(scriptPath))
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                scriptPath = Path.Combine(assemblyLocation!, "Resources", "loader.py");
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
