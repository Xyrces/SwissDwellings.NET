using System.Text.Json.Serialization;

namespace SwissDwellings.Data
{
    public class MsdEdge
    {
        [JsonPropertyName("source")]
        public object Source { get; set; } = new object();

        [JsonPropertyName("target")]
        public object Target { get; set; } = new object();

        // To hold other attributes
        [JsonExtensionData]
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    }
}
