using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using SwissDwellings;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Benchmark...");

        // Get the field info
        var type = typeof(SwissDwellingLoader);
        var field = type.GetField("_defaultPythonExecutable", BindingFlags.Static | BindingFlags.NonPublic);

        if (field == null)
        {
            Console.WriteLine("Could not find field _defaultPythonExecutable");
            return;
        }

        int iterations = 5;
        long totalMs = 0;

        // Create a dummy file to pass as path, so we skip DataManager.EnsureDataAsync
        string dummyPath = "dummy.json";
        await System.IO.File.WriteAllTextAsync(dummyPath, "[]");

        try
        {
            for (int i = 0; i < iterations; i++)
            {
                // Reset the field to force detection
                field.SetValue(null, null);

                var sw = Stopwatch.StartNew();

                try
                {
                    // Call LoadLayoutsAsync.
                    // This triggers GetDefaultPythonExecutableAsync().
                    // It will likely fail later because the default script is not found or fails,
                    // but we only care about the time it takes to get to that point (which includes detection).
                    await SwissDwellingLoader.LoadLayoutsAsync(path: dummyPath);
                }
                catch
                {
                    // Ignore exceptions (likely FileNotFound for script or InvalidOperation for script execution)
                }

                sw.Stop();
                Console.WriteLine($"Iteration {i + 1}: {sw.ElapsedMilliseconds} ms");
                totalMs += sw.ElapsedMilliseconds;
            }

            Console.WriteLine($"Average time: {totalMs / iterations} ms");
        }
        finally
        {
            if (System.IO.File.Exists(dummyPath)) System.IO.File.Delete(dummyPath);
        }
    }
}
