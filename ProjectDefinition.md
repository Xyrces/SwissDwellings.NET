Project Definition: SwissDwellings.NET
Type: Open Source Library (NuGet)
Repository Name: SwissDwellings.NET
Namespace: SwissDwellings
Purpose: A pure .NET parser for the Modified Swiss Dwellings (MSD) dataset, focusing on Graph deserialization.

1. Project Scope
This library reconstructs the NetworkX graphs provided in MSD JSON files into strongly-typed C# objects.

IN SCOPE: Deserializing JSON nodes/links, Mapping topological attributes (Room Type, Zoning), Geometry extraction.
OUT OF SCOPE: Vertical portal inference (Stair guessing), MapGenerator integration, Schematic normalization.
2. Dependencies
.NET 9
System.Text.Json: For high-performance JSON parsing.
NetTopologySuite: For geometry.
QuikGraph (Optional): Or a custom lightweight Graph implementation.
3. Public API Contract
3.1 Data Models
namespace SwissDwellings.Data
{
    public class MsdBuilding
    {
        public string Id { get; set; }
        public MsdGraph Graph { get; set; }
        public List<MsdUnit> Units { get; set; }
    }
    // ... (MsdUnit, MsdGraph, MsdNode, MsdEdge models)
    // Same as previous definition, strictly representing raw data structure.
}
3.2 Service Interface
namespace SwissDwellings
{
    public interface IMsdLoader
    {
        // Loads a complete MSD dataset entry
        MsdBuilding Load(string jsonFilePath);
        
        // Returns only the structural graph (ignoring unit details)
        MsdGraph LoadStructureOnly(string jsonFilePath);
    }
}
4. Implementation Guidelines
JSON Schema: MSD uses a specific NetworkX node-link structure ({"nodes": [], "links": []}). The loader must match this exactly.
Robustness: Some MSD entries may have disjoint graphs. The loader should accept valid subgraphs rather than enforcing full connectedness.
Deliverable: A compiled SwissDwellings.dll and Unit Tests.
