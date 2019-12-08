using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Model
{
    public class QueryResult
    {
        public IReadOnlyList<JToken> Items { get; set; } = Array.Empty<JToken>();
        public Exception? Error { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public double RequestCharge { get; set; }
        public string? ContinuationToken { get; set; }
        public IEnumerable<string> Warnings { get; set; } = Array.Empty<string>();
    }
}
