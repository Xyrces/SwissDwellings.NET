using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace SwissDwellings.Data
{
    public class MsdNode
    {
        [JsonPropertyName("id")]
        public object Id { get; set; } = new object(); // NetworkX ids can be strings or ints

        [JsonPropertyName("room_type")]
        public string? RoomType { get; set; }

        [JsonPropertyName("zoning")]
        public string? Zoning { get; set; }

        // Geometry will need to be parsed from a string or object in the JSON
        // We will likely handle this in the Loader or via a custom converter if it's complex.
        // For now, let's assume there is a property "geometry" in the node attributes.
        // However, since we are using System.Text.Json, we might need to expose the raw string
        // or have the loader populate this property.
        // If property name matches JSON key, [JsonIgnore] might prevent it from going to ExtensionData.
        // Rename property to avoid conflict.
        [JsonIgnore]
        public Geometry? ParsedGeometry { get; set; }

        // This is where "geometry" from JSON might go if we don't have a matching property
        // But wait, I want it in Attributes.

        // To hold other attributes
        [JsonExtensionData]
        public Dictionary<string, object>? Attributes { get; set; }
    }

    // Custom converter might be needed if JsonExtensionData is not behaving as expected with System.Text.Json in .NET 9 for some reason
    // But it should work.

    // Let's try to add a constructor to initialize Attributes? No, I tried that.
    // Maybe the issue is that "geometry" is considered a property if I have it defined?
    // But I have [JsonIgnore] on Geometry property.
    // Wait, if I have [JsonIgnore] public Geometry Geometry { get; set; }, does System.Text.Json put "geometry" from JSON into Attributes?
    // Yes, it should, because "geometry" does not match any ignored property name?
    // Actually, [JsonIgnore] means "ignore this property". If the JSON has "geometry", it looks for a property named "geometry".
    // If it finds one and it is ignored, it might skip it and NOT put it in ExtensionData?
    // "If a property is ignored, the JSON value is not deserialized into that property. If the type has an extension data property, the value is added to the extension data."
    // Let's verify this behavior.

}
