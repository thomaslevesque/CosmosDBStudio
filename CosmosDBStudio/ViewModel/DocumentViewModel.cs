using CosmosDBStudio.Extensions;
using CosmosDBStudio.Services;
using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

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
            if (document is JObject obj)
            {
                if (obj.TryGetValue("id", out var idToken))
                {
                    HasId = true;
                    FirstColumnTitle = "id";
                    DisplayValue = Id = idToken.Value<string>();
                }
                else
                {
                    FirstColumnTitle = "value";
                    var props = obj.Properties().ToList();
                    if (props.Count < 2)
                    {
                        DisplayValue = obj.ToString(Formatting.None);
                    }
                    else
                    {
                        var firstProp = props.First();
                        DisplayValue = $"{{{firstProp.ToString(Formatting.None)}, …}}";
                    }
                }
            }
            else if (document is JValue value)
            {
                FirstColumnTitle = "value";
                DisplayValue = value.Value?.ToString() ?? "(null)";
            }
            else if (document is JArray array)
            {
                FirstColumnTitle = "values";

                var firstItems = new JArray(array.Take(2));
                if (array.Count > 2)
                    firstItems.Add(new JRaw("…"));
                DisplayValue = firstItems.ToString(Formatting.None);
            }
            else
            {
                // Should never happen
                throw new InvalidOperationException("Unexpected document type");
            }

            if (!string.IsNullOrEmpty(containerContext.PartitionKeyJsonPath))
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

        public override string DisplayValue { get; }
        public override object? PartitionKey { get; }
        public override bool IsJson => true;

        public string? Id { get; }
        public bool HasId { get; }
        public bool HasPartitionKey { get; }
        public string? ETag { get; }
        public bool IsRawDocument { get; }
        public string FirstColumnTitle { get; }

        private string _text;
        public override string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public JToken GetDocument() => _document;
    }
}
