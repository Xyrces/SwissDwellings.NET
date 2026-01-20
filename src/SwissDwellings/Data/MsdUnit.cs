using System.Collections.Generic;

namespace SwissDwellings.Data
{
    public class MsdUnit
    {
        public string Id { get; set; } = string.Empty;

        // Assuming unit might reference nodes
        public List<object> NodeIds { get; set; } = new List<object>();
    }
}
