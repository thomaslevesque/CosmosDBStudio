using Hamlet;
using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class Query
    {
        public Query(string sql)
        {
            Sql = sql;
            Parameters = new Dictionary<string, object?>();
        }

        public string Sql { get; set; }
        public Option<object?> PartitionKey { get; set; }
        public IDictionary<string, object?> Parameters { get; set; }
    }
}
