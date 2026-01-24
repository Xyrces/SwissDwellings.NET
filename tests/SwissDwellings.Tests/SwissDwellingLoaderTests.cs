using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SwissDwellings.Data;
using Xunit;
using FluentAssertions;

namespace SwissDwellings.Tests
{
    public class SwissDwellingLoaderTests
    {
        [Fact]
        public async Task LoadLayoutsAsync_ReturnsLayouts_WhenScriptExecutedSuccessfully()
        {
            // Arrange
            var scriptPath = Path.GetTempFileName() + ".py";
            var dataPath = Path.GetTempFileName(); // Dummy data path

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
            await File.WriteAllTextAsync(scriptPath, pythonCode);
            await File.WriteAllTextAsync(dataPath, "dummy data");

            try
            {
                // Act
                // Use the internal overload to test with a specific script
                var result = await SwissDwellingLoader.LoadLayoutsInternalAsync(dataPath, scriptPath);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(1);
                result[0].Id.Should().Be("test_id");
            }
            finally
            {
                // Cleanup
                if (File.Exists(scriptPath)) File.Delete(scriptPath);
                if (File.Exists(dataPath)) File.Delete(dataPath);
            }
        }
    }
}
