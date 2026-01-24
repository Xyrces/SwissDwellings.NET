using System.Text.Json.Serialization;

namespace SwissDwellings.Data
{
    public class SwissDwellingLayout
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("graph")]
        public MsdGraph Graph { get; set; } = new MsdGraph();
    }
}
