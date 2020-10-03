using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace CosmosDBStudio.Model
{
    public class StoredProcedureResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public JToken? Body { get; set; }
        public Exception? Error { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public double RequestCharge { get; set; }
        public string ScriptLog { get; set; } = string.Empty;
    }
}
