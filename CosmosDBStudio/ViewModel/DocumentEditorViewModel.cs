using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Extensions;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class DocumentEditorViewModel : DialogViewModelBase, ISizableDialog
    {
        private readonly IContainerContext _containerContext;
        private readonly Timer _validateJsonTimer;

        private JObject? _document;

        public DocumentEditorViewModel(IContainerContext containerContext, JObject? document)
        {
            _containerContext = containerContext;
            _validateJsonTimer = new Timer(
                    state => ((DocumentEditorViewModel)state!).ValidateJson(),
                    this,
                    Timeout.Infinite,
                    Timeout.Infinite);

            if (document is null)
            {
                Title = "New document";
                _text = "{}";
            }
            else
            {
                Title = "Edit document";
                _document = document;
                _id = document["id"].Value<string>();
                _eTag = document["_etag"]?.Value<string>();
                _text = document.ToString(Formatting.Indented);
            }
        }

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndExecute(InvalidateJson)
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private AsyncDelegateCommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncDelegateCommand(
            SaveAsync,
            () => IsJsonValid is true);

        private bool? _isJsonValid;
        public bool? IsJsonValid
        {
            get => _isJsonValid;
            set => Set(ref _isJsonValid, value)
                .AndNotifyPropertyChanged(nameof(IsError));
        }

        private void InvalidateJson()
        {
            IsJsonValid = null;
            _validateJsonTimer.Change(500, Timeout.Infinite);
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
            get => _isError || (IsJsonValid is false);
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

        private double _width = 500;
        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        private double _height = 500;
        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public bool IsResizable => true;

        public override void OnClosed(bool? result)
        {
            base.OnClosed(result);
            _validateJsonTimer.Dispose();
        }
    }
}
