using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public class DocumentViewModel : BindableBase
    {
        public DocumentViewModel(JObject document)
        {
            HasId = document.TryGetValue("id", out var token);
            if (HasId)
                Id = token.Value<string>();
            else
                Id = "(no id)";

            _json = document.ToString(Formatting.Indented);
        }

        private DocumentViewModel(string idText, bool isError)
        {
            Id = idText;
            IsError = isError;
            IsReadOnly = true;
        }

        public bool IsError { get; set; }
        public bool IsReadOnly { get; set; }
        public bool HasId { get; }
        public string Id { get; }

        private string _json;
        public string Json
        {
            get => _json;
            set => Set(ref _json, value);
        }

        public static DocumentViewModel Error() => new DocumentViewModel("(Error - see Error tab for details)", true);
        public static DocumentViewModel NoResults() => new DocumentViewModel("(no results)", false);
    }
}
