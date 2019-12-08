using Newtonsoft.Json;

namespace CosmosDBStudio.Model
{
    public class QuerySheet
    {
        [JsonIgnore]
        public string? Path { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string DatabaseId { get; set; } = string.Empty;
        public string ContainerId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public QueryOptions DefaultOptions { get; set; } = new QueryOptions();
    }
}
