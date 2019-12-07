using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{

    public class DocumentViewModel : ResultItemViewModel
    {
        private readonly JToken _document;

        public DocumentViewModel(JToken document)
        {
            _document = document;
            JToken idToken = null;
            bool hasId = document is JObject obj && obj.TryGetValue("id", out idToken);
            if (hasId)
                Id = idToken.Value<string>();
            else if (document is JValue value)
                Id = value.Value?.ToString() ?? "(null)";
            else
                Id = "(no id)";

            IsReadOnly = !IsRawDocument(document);
            _text = document.ToString(Formatting.Indented);
        }

        public override string DisplayId => Id;
        public override string PartitionKey => throw new System.NotImplementedException();
        public override bool IsReadOnly { get; }
        public override bool IsJson => true;

        public string Id { get; }

        private string _text;
        public override string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        private static bool IsRawDocument(JToken document)
        {
            // If the document has all system properties, it's probably
            // a raw document, and can be edited
            return document is JObject obj &&
                obj.TryGetValue("id", out _) &&
                obj.TryGetValue("_rid", out _) &&
                obj.TryGetValue("_self", out _) &&
                obj.TryGetValue("_etag", out _) &&
                obj.TryGetValue("_ts", out _);
        }
    }
}
