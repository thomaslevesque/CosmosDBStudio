using CosmosDBStudio.Extensions;
using CosmosDBStudio.Services;
using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{

    public class DocumentViewModel : ResultItemViewModel
    {
        private readonly JToken _document;
        private readonly IContainerContext _containerContext;

        public DocumentViewModel(JToken document, IContainerContext containerContext)
        {
            _document = document;
            _containerContext = containerContext;
            JToken? idToken = null;
            HasId = document is JObject obj && obj.TryGetValue("id", out idToken);
            if (HasId)
                Id = idToken.Value<string>();
            else if (document is JValue value)
                Id = value.Value?.ToString() ?? "(null)";
            else
                Id = "(no id)";

            if (HasId && !string.IsNullOrEmpty(containerContext.PartitionKeyJsonPath))
            {
                var pk = document.ExtractScalar(containerContext.PartitionKeyJsonPath);
                HasPartitionKey = pk.IsSome;
                PartitionKey = pk.ValueOrNull();
            }

            IsRawDocument = document.IsRawDocument();
            if (IsRawDocument)
            {
                var eTagToken = ((JObject)document).GetValue("_etag") as JValue;
                ETag = eTagToken?.Value<string?>();
            }
            _text = document.ToString(Formatting.Indented);
        }

        public override string DisplayId => Id;
        public override object? PartitionKey { get; }
        public override bool IsJson => true;

        public string Id { get; }
        public bool HasId { get; }
        public bool HasPartitionKey { get; }
        public string? ETag { get; }
        public bool IsRawDocument { get; }

        private string _text;
        public override string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public JToken GetDocument() => _document;
    }
}
