using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SwissDwellings.Data
{
    public class MsdBuilding
    {
        public string Id { get; set; } = string.Empty;

        // Assuming the structure has a "graph" property or we construct it.
        // If the JSON is flat NetworkX, we might need a custom converter or the structure is different.
        // For now, I'll assume the loaded object is MsdBuilding and it contains MsdGraph.
        public MsdGraph Graph { get; set; } = new MsdGraph();

        public List<MsdUnit> Units { get; set; } = new List<MsdUnit>();
    }
}
