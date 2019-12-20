using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class QuerySheet
    {
        public string AccountId { get; set; } = string.Empty;
        public string DatabaseId { get; set; } = string.Empty;
        public string ContainerId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? PartitionKey { get; set; }
        public IDictionary<string, string?> Parameters { get; set; } = new Dictionary<string, string?>();
    }
}
