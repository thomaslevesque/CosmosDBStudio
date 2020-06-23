using Hamlet;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Extensions
{
    public static class JTokenExtensions
    {
        public static Option<object?> ExtractScalar(this JToken document, string jsonPath)
        {
            var token = document.SelectToken(jsonPath);
            return token is JValue valueToken
                ? Option.Some((object?)valueToken.Value)
                : Option.None();
        }

        public static bool IsRawDocument(this JToken document)
        {
            // If the document has all system properties, it's probably
            // a raw document
            return document is JObject obj &&
                obj.TryGetValue("id", out _) &&
                obj.TryGetValue("_rid", out _) &&
                obj.TryGetValue("_self", out _) &&
                obj.TryGetValue("_etag", out _) &&
                obj.TryGetValue("_ts", out _);
        }
    }
}
