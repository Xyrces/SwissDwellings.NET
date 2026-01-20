using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SwissDwellings.Data
{
    public class MsdGraph
    {
        [JsonPropertyName("nodes")]
        public List<MsdNode> Nodes { get; set; } = new List<MsdNode>();

        [JsonPropertyName("links")]
        public List<MsdEdge> Edges { get; set; } = new List<MsdEdge>();

        // NetworkX graphs can have graph-level attributes
        [JsonPropertyName("graph")]
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("directed")]
        public bool Directed { get; set; }

        [JsonPropertyName("multigraph")]
        public bool Multigraph { get; set; }
    }
}
