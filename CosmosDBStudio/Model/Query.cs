using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class Query
    {
        public string ConnectionId { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public QueryOptions Options { get; set; }
        public string ContinuationToken { get; set; }
    }
}
