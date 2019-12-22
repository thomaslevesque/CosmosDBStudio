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
        private readonly IUIDispatcher _uiDispatcher;
        private readonly Timer _validateJsonTimer;

        private JObject? _document;
        private string? _id;
        private string? _eTag;

        public DocumentEditorViewModel(
            JObject? document,
            IContainerContext containerContext,
            IUIDispatcher uiDispatcher)
        {
            _containerContext = containerContext;
            _uiDispatcher = uiDispatcher;
            _validateJsonTimer = new Timer(
                    state =>
                    {
                        var @this = ((DocumentEditorViewModel)state!);
                        @this._uiDispatcher.Invoke(() => @this.ValidateJson());
                    },
                    this,
                    Timeout.Infinite,
                    Timeout.Infinite);

            if (document is null)
            {
                Title = "New document";
                _document = new JObject();
                _document["id"] = Guid.NewGuid();
                _text = _document.ToString(Formatting.Indented);
            }
            else
            {
                Title = "Edit document";
                _document = document;
                _id = document["id"].Value<string>();
                _eTag = document["_etag"]?.Value<string>();
                _text = document.ToString(Formatting.Indented);
            }

            base.AddButton(new DialogButton
            {
                Text = "Close",
                Command = new DelegateCommand(() => Close(null)),
                IsCancel = true
            });
        }

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndExecute(InvalidateJson)
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        // Static so that it's remembered between doc edits
        private static bool _closeOnSave = true;
        public bool CloseOnSave
        {
            get => _closeOnSave;
            set => Set(ref _closeOnSave, value);
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
                .AndNotifyPropertyChanged(nameof(IsError))
                .AndRaiseCanExecuteChanged(_saveCommand);
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

                    _id = result["id"].Value<string>();
                    _eTag = result["_etag"]?.Value<string?>();

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
                    
                    StatusText = "Successfully created";
                }
                _text = result.ToString(Formatting.Indented);
                _document = result;
                IsError = false;
                OnPropertyChanged(nameof(Text));
                _saveCommand?.RaiseCanExecuteChanged();
                if (CloseOnSave)
                {
                    Close(true);
                }
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
