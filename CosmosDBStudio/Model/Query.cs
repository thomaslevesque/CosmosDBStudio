using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class Query
    {
        public Query(string sql)
        {
            Sql = sql;
            Parameters = new Dictionary<string, object>();
            Options = new QueryOptions();
        }

        public string Sql { get; set; }
        public IDictionary<string, object?> Parameters { get; set; }
        public QueryOptions Options { get; set; }
        public string? ContinuationToken { get; set; }
    }
}
