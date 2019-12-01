using Newtonsoft.Json;

namespace CosmosDBStudio.Model
{
    public class QuerySheet
    {
        [JsonIgnore]
        public string Path { get; set; }
        public string ConnectionId { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
        public string Text { get; set; }
        public QueryOptions DefaultOptions { get; set; }
    }
}
