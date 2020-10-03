using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CosmosDBStudio.Helpers
{
    public class JsonHelper
    {
        public static bool TryParseJsonValue(string? rawJson, out object? value)
        {
            if (string.IsNullOrEmpty(rawJson))
            {
                value = null;
                return false;
            }

            try
            {
                using var tReader = new StringReader(rawJson);
                using var jReader = new JsonTextReader(tReader)
                {
                    DateParseHandling = DateParseHandling.None
                };

                var token = JValue.ReadFrom(jReader);
                value = token.ToObject<object?>();
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static Option<object?> TryParseJsonValue(string? rawJson)
        {
            return TryParseJsonValue(rawJson, out var value)
                ? Option.Some(value)
                : Option.None();
        }
    }
}
