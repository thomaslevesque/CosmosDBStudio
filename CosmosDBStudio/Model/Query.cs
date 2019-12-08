using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class Query
    {
        public Query(string accountId, string databaseId, string containerId, string sql)
        {
            AccountId = accountId;
            DatabaseId = databaseId;
            ContainerId = containerId;
            Sql = sql;
            Parameters = new Dictionary<string, object>();
            Options = new QueryOptions();
        }

        public string AccountId { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public QueryOptions Options { get; set; }
        public string? ContinuationToken { get; set; }
    }
}
