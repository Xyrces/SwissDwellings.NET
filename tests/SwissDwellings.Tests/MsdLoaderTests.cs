using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using NetTopologySuite.Geometries;
using SwissDwellings.Data;
using Xunit;

namespace SwissDwellings.Tests
{
    public class MsdLoaderTests
    {
        [Fact]
        public void Load_ReturnsBuilding_FromValidJson()
        {
            // Arrange
            var json = @"{
                ""nodes"": [
                    { ""id"": ""n1"" },
                    { ""id"": ""n2"" }
                ],
                ""links"": [
                    { ""source"": ""n1"", ""target"": ""n2"" }
                ],
                ""graph"": {
                    ""id"": ""building_123"",
                    ""units"": []
                }
            }";

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, json);

            var loader = new MsdLoader();

            try
            {
                // Act
                MsdBuilding result = loader.Load(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be("building_123");
                result.Graph.Should().NotBeNull();
                result.Graph.Nodes.Should().HaveCount(2);
                result.Graph.Edges.Should().HaveCount(1);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_ParsesAttributesAndGeometry()
        {
            // Arrange
            var json = @"{
                ""nodes"": [
                    {
                        ""id"": ""room1"",
                        ""room_type"": ""Bedroom"",
                        ""zoning"": ""Private"",
                        ""geometry"": ""POINT (10 20)""
                    }
                ],
                ""links"": [],
                ""graph"": {}
            }";

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, json);

            var loader = new MsdLoader();

            try
            {
                // Act
                MsdBuilding result = loader.Load(tempFile);

                // Assert
                result.Graph.Nodes.Should().HaveCount(1);
                var node = result.Graph.Nodes[0];
                node.RoomType.Should().Be("Bedroom");
                node.Zoning.Should().Be("Private");

                node.ParsedGeometry.Should().NotBeNull();
                node.ParsedGeometry.Should().BeOfType<Point>();
                node.ParsedGeometry.Coordinate.X.Should().Be(10);
                node.ParsedGeometry.Coordinate.Y.Should().Be(20);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        [Fact]
        public void LoadStructureOnly_ReturnsGraph_WithoutBuildingWrapper()
        {
            // Arrange
            var json = @"{
                ""nodes"": [
                    { ""id"": ""n1"" }
                ],
                ""links"": [],
                ""graph"": {
                    ""id"": ""building_123"",
                    ""units"": []
                }
            }";

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, json);

            var loader = new MsdLoader();

            try
            {
                // Act
                MsdGraph result = loader.LoadStructureOnly(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Nodes.Should().HaveCount(1);
                // We expect attributes to be populated
                result.Attributes.Should().ContainKey("id");
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}
