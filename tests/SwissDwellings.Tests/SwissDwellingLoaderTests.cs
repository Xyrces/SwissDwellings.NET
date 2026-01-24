using System;
using System.IO;
using System.Threading.Tasks;
using SwissDwellings.Data;
using Xunit;
using FluentAssertions;

namespace SwissDwellings.Tests
{
    public class SwissDwellingLoaderTests : IAsyncLifetime
    {
        private string _scriptPath;
        private string _dataPath;

        public async Task InitializeAsync()
        {
            _scriptPath = Path.GetTempFileName() + ".py";
            _dataPath = Path.GetTempFileName(); // Dummy data path

            // Create a simple python script that prints JSON to stdout
            var pythonCode = @"
import json
import sys

# The output should match List<SwissDwellingLayout>
data = [
    {
        ""id"": ""test_id"",
        ""graph"": {
            ""nodes"": [],
            ""links"": [],
            ""directed"": False,
            ""multigraph"": False,
            ""graph"": {}
        }
    }
]
print(json.dumps(data))
";
            await File.WriteAllTextAsync(_scriptPath, pythonCode);
            await File.WriteAllTextAsync(_dataPath, "dummy data");

            // Override the script path for testing
            SwissDwellingLoader.ScriptOverride = _scriptPath;
        }

        public async Task DisposeAsync()
        {
             // Cleanup
             SwissDwellingLoader.ScriptOverride = null;

            if (File.Exists(_scriptPath)) File.Delete(_scriptPath);
            if (File.Exists(_dataPath)) File.Delete(_dataPath);
            await Task.CompletedTask;
        }

        [Fact]
        public async Task LoadLayoutsAsync_ReturnsLayouts_WhenScriptExecutedSuccessfully()
        {
            // Act
            // We pass the data path, but the script path is handled via ScriptOverride
            var result = await SwissDwellingLoader.LoadLayoutsAsync(_dataPath);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be("test_id");
        }
    }
}
