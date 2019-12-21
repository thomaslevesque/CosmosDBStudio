using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Extensions;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class DocumentEditorViewModel : DialogViewModelBase
    {
        private readonly IContainerContext _containerContext;

        private JObject? _document;
        public DocumentEditorViewModel(IContainerContext containerContext, JObject? document)
        {
            _containerContext = containerContext;
            if (document is null)
            {
                Title = "New document";
                Text = "{}";
            }
            else
            {
                Title = "Edit document";
                _document = document;
                _id = document["id"].Value<string>();
                _eTag = document["_etag"]?.Value<string>();
                Text = document.ToString(Formatting.Indented);
            }
        }

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndExecute(ValidateJson)
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private AsyncDelegateCommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncDelegateCommand(
            SaveAsync,
            () => IsJsonValid);

        private bool _isJsonValid;
        public bool IsJsonValid
        {
            get => _isJsonValid;
            set => Set(ref _isJsonValid, value)
                .AndNotifyPropertyChanged(nameof(IsError));
        }

        private void ValidateJson()
        {
            try
            {
                JObject.Parse(Text, new JsonLoadSettings
                {
                    LineInfoHandling = LineInfoHandling.Load
                });
                IsJsonValid = true;
                StatusText = null;
            }
            catch (JsonReaderException ex)
            {
                IsJsonValid = false;
                StatusText = $"{ex.Message} at path '{ex.Path}' (line {ex.LineNumber}, position {ex.LinePosition})";
            }
        }

        private string? _statusText;
        public string? StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError || !IsJsonValid;
            set => Set(ref _isError, value);
        }

        private string? _id;
        private string? _eTag;

        private async Task SaveAsync()
        {
            var doc = JObject.Parse(Text);

            var partitionKey =
                _containerContext.PartitionKeyJsonPath is string jsonPath
                ? doc.ExtractScalar(jsonPath)
                : Option.None();

            try
            {
                JObject result;
                if (_id is string id)
                {
                    result = await _containerContext.Documents.ReplaceAsync(
                        id,
                        doc,
                        partitionKey,
                        _eTag,
                        default);

                    _eTag = result["_etag"]?.Value<string?>();

                    Text = result.ToString(Formatting.Indented);
                    StatusText = "Successfully updated";
                }
                else
                {
                    result = await _containerContext.Documents.CreateAsync(
                        doc,
                        partitionKey,
                        default);

                    _id = result["id"].Value<string>();
                    _eTag = result["_etag"]?.Value<string?>();

                    Text = result.ToString(Formatting.Indented);
                    StatusText = "Successfully created";
                }
                _document = result;
                IsError = false;
            }
            catch (Exception ex)
            {
                IsError = true;
                StatusText = ex.Message;
            }
        }

        public JObject? GetDocument() => _document;
    }
}
