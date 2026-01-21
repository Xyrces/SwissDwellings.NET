using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SwissDwellings.Data;

namespace SwissDwellings
{
    public class MsdLoader : IMsdLoader
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        private static readonly WKTReader _wktReader = new WKTReader();

        public MsdBuilding Load(string jsonFilePath)
        {
            using var stream = File.OpenRead(jsonFilePath);
            var graph = JsonSerializer.Deserialize<MsdGraph>(stream, _options);

            if (graph == null) throw new InvalidOperationException("Failed to deserialize graph.");

            ParseGeometries(graph);

            var building = new MsdBuilding
            {
                Graph = graph
            };

            if (graph.Attributes != null)
            {
                if (graph.Attributes.TryGetValue("id", out var idObj) && idObj is JsonElement idEl)
                {
                    building.Id = idEl.ToString();
                }

                if (graph.Attributes.TryGetValue("units", out var unitsObj) && unitsObj is JsonElement unitsEl)
                {
                    building.Units = JsonSerializer.Deserialize<List<MsdUnit>>(unitsEl.GetRawText(), _options) ?? new List<MsdUnit>();
                }
            }

            return building;
        }

        public MsdGraph LoadStructureOnly(string jsonFilePath)
        {
             using var stream = File.OpenRead(jsonFilePath);
             var graph = JsonSerializer.Deserialize<MsdGraph>(stream, _options);

             if (graph == null) throw new InvalidOperationException("Failed to deserialize graph.");

             ParseGeometries(graph);

             return graph;
        }

        private void ParseGeometries(MsdGraph graph)
        {
            if (graph.Nodes != null)
            {
                foreach (var node in graph.Nodes)
                {
                    if (node.Attributes != null)
                    {
                        if (node.Attributes.TryGetValue("geometry", out var geomObj))
                        {
                            if (geomObj is JsonElement geomEl && geomEl.ValueKind == JsonValueKind.String)
                            {
                                try
                                {
                                    node.ParsedGeometry = _wktReader.Read(geomEl.GetString());
                                }
                                catch
                                {
                                    // Ignore or log invalid geometry
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
