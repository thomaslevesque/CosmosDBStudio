using CosmosDBStudio.Extensions;
using CosmosDBStudio.Helpers;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class QuerySheetViewModel : TabViewModelBase, ISaveable
    {
        public static int UntitledCounter { get; set; }

        private readonly IViewModelFactory _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly IMessenger _messenger;

        private IContainerContext? _containerContext;
        private readonly IQueryPersistenceService _queryPersistenceService;

        public QuerySheetViewModel(
            QuerySheet querySheet,
            string? path,
            IContainerContext? containerContext,
            IViewModelFactory viewModelFactory,
            IDialogService dialogService,
            IMessenger messenger,
            IQueryPersistenceService queryPersistenceService)
        {
            _containerContext = containerContext;
            _queryPersistenceService = queryPersistenceService;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _messenger = messenger;
            _filePath = path;
            Title = string.IsNullOrEmpty(path)
                ? "Untitled " + (++UntitledCounter)
                : Path.GetFileNameWithoutExtension(path);
            _text = querySheet.Text;
            _result = _viewModelFactory.CreateNotRunQueryResult();

            PartitionKey = querySheet.PartitionKey;
            PartitionKeyMRU = new ObservableCollection<string>();
            foreach (var mru in querySheet.PartitionKeyMRU)
            {
                PartitionKeyMRU.Add(mru);
            }

            Parameters = new ParametersViewModel<QueryParameterViewModel>();
            foreach (var p in querySheet.Parameters)
            {
                Parameters.AddParameter(CreateParameter(p));
                ShowParameters = true;
            }

            Parameters.AddPlaceholder();

            Errors = new ViewModelValidator<QuerySheetViewModel>(this);
            Errors.AddValidator(
                vm => vm.PartitionKey,
                value => string.IsNullOrEmpty(value) || JsonHelper.TryParseJsonValue(value, out _)
                    ? null
                    : "Invalid partition key value");

            _messenger.Subscribe(this).To<ExplorerSelectedContainerChangedMessage>(
                (vm, message) => vm.OnExplorerSelectedContainerChanged(message));
        }

        public ViewModelValidator<QuerySheetViewModel> Errors { get; }

        public override string Title { get; }

        public override string Description => "query sheet";

        private string? _filePath;
        public string? FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value).AndNotifyPropertyChanged(nameof(Title));
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndRaiseCanExecuteChanged(_executeCommand)
                .AndExecute(() => SetHasChanges(true));
        }

        public string ContainerPath => _containerContext?.Path ?? "(no container selected)";

        private string _selectedText = string.Empty;
        public string SelectedText
        {
            get => _selectedText;
            set => Set(ref _selectedText, value)
                .AndRaiseCanExecuteChanged(_executeCommand);
        }

        private (int start, int end) _selection;
        public (int start, int end) Selection
        {
            get => _selection;
            set => Set(ref _selection, value);
        }

        private int _cursorPosition;
        public int CursorPosition
        {
            get => _cursorPosition;
            set => Set(ref _cursorPosition, value)
                .AndRaiseCanExecuteChanged(_executeCommand);
        }

        public ObservableCollection<string> PartitionKeyMRU { get; }

        private string? _partitionKey;
        public string? PartitionKey
        {
            get => _partitionKey;
            set => Set(ref _partitionKey, value).AndExecute(() => Errors?.Refresh());
        }

        public ParametersViewModel<QueryParameterViewModel> Parameters { get; }

        private QueryResultViewModelBase _result;
        public QueryResultViewModelBase Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        private AsyncDelegateCommand? _executeCommand;
        public ICommand ExecuteCommand => _executeCommand ??= new AsyncDelegateCommand(ExecuteAsync, CanExecute);

        private bool _showParameters;
        public bool ShowParameters
        {
            get => _showParameters;
            set => Set(ref _showParameters, value);
        }

        public QuerySheet GetQuerySheet()
        {
            var querySheet = new QuerySheet
            {
                Text = Text,
                PartitionKey = PartitionKey,
                PartitionKeyMRU = PartitionKeyMRU.ToList()
            };

            foreach (var p in Parameters.Parameters)
            {
                if (p.Name is string name)
                {
                    querySheet.Parameters.Add(new QuerySheetParameter
                    {
                        Name = name,
                        RawValue = p.RawValue,
                        MRU = p.MRU.ToList()
                    });
                }
            }

            return querySheet;
        }

        private bool CanExecute()
        {
            return (_containerContext is { }) &&
                !Errors.HasError &&
                (!string.IsNullOrEmpty(SelectedText) ||
                 !string.IsNullOrEmpty(ExtendSelectionAroundCursor(false)));
        }

        private async Task ExecuteAsync()
        {
            if (!(_containerContext is IContainerContext containerContext))
                return;

            var queryText = SelectedText;
            if (string.IsNullOrEmpty(queryText))
                queryText = ExtendSelectionAroundCursor(true);

            if (string.IsNullOrEmpty(queryText))
                return;

            // TODO: options

            Option<object?> partitionKey = Option.None();
            if (!string.IsNullOrEmpty(PartitionKey) && JsonHelper.TryParseJsonValue(PartitionKey, out object? pkValue))
            {
                partitionKey = pkValue;
                PartitionKeyMRU.PushMRU(PartitionKey, 10);
            }

            var query = new Query(queryText);
            query.PartitionKey = partitionKey;
            foreach (var p in Parameters.Parameters)
            {
                if (p.IsPlaceholder || p.Errors.HasError)
                    continue;

                string name = p.Name!;
                string nakedName;
                if (name.StartsWith('@'))
                {
                    nakedName = name.Substring(1);
                }
                else
                {
                    nakedName = name;
                    name = "@" + name;
                }

                if (!Regex.IsMatch(queryText, $@"@\b{nakedName}\b", RegexOptions.Multiline))
                    continue;

                p.TryParseParameterValue(out object? value);
                query.Parameters[name] = value;
                p.MRU.PushMRU(p.RawValue!, 10);
            }

            QueryResult? result;
            try
            {
                IsQueryRunning = true;
                _messenger.Publish(new SetStatusBarMessage("Executing query..."));
                result = await containerContext.Query.ExecuteAsync(query, null, default);
                _messenger.Publish(new SetStatusBarMessage($"Query executed in {result.TimeElapsed}. Request charge: {result.RequestCharge} RU/s"));
            }
            finally
            {
                IsQueryRunning = false;
            }

            Result = _viewModelFactory.CreateQueryResult(result, containerContext);
        }

        private static readonly string QuerySeparator = Environment.NewLine + Environment.NewLine;
        private string ExtendSelectionAroundCursor(bool applySelectionChange)
        {
            if (string.IsNullOrEmpty(Text))
                return string.Empty;

            var position = CursorPosition;
            if (position > Text.Length)
                position = Text.Length;

            var previousSeparator = Text.LastIndexOf(QuerySeparator, position, position);
            if (previousSeparator < 0)
                previousSeparator = 0;

            var nextSeparator = Text.IndexOf(QuerySeparator, position);
            if (nextSeparator < 0)
                nextSeparator = Text.Length - 1;

            int start = ForceIndexInRange(previousSeparator);
            while (char.IsWhiteSpace(Text[start]) && start + 1 < Text.Length)
                start++;

            int end = ForceIndexInRange(nextSeparator);
            while (char.IsWhiteSpace(Text[end]) && end - 1 >= 0)
                end--;

            if (start > end)
                return string.Empty;

            string queryText = Text.Substring(start, end - start + 1);
            //string queryText = Text.Substring(previousSeparator, nextSeparator - previousSeparator).Trim();
            if (queryText.Contains(QuerySeparator))
            {
                // We have two queries; the cursor was probably in the middle of the separator
                return string.Empty;
            }

            if (applySelectionChange)
            {
                Selection = (start, end - start + 1);
            }

            return queryText;

            int ForceIndexInRange(int index)
            {
                if (index < 0)
                    return 0;
                if (index >= Text.Length)
                    return Text.Length - 1;
                return index;
            }
        }

        private DelegateCommand? _newDocumentCommand;
        public ICommand NewDocumentCommand => _newDocumentCommand ??= new DelegateCommand(NewDocument, CanCreateNewDocument);

        private bool CanCreateNewDocument() => _containerContext is { };

        private void NewDocument()
        {
            if (!(_containerContext is IContainerContext containerContext))
                return;

            var document = new JObject();
            document["id"] = Guid.NewGuid().ToString();
            SetPartitionKey(document, PartitionKey);
            var vm = _viewModelFactory.CreateDocumentEditor(document, true, containerContext);
            _dialogService.ShowDialog(vm);
        }

        /// <summary>
        /// Sets the partition key on a new document, based on the currently
        /// selected one, if any. If no partition key is selected, add the
        /// partition key property with a default value so that the user
        /// remembers to set it.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="partitionKeyRawValue"></param>
        private void SetPartitionKey(JObject document, string? partitionKeyRawValue)
        {
            if (!(_containerContext is IContainerContext containerContext))
                return;

            if (string.IsNullOrEmpty(containerContext.PartitionKeyPath))
                return;

            if (containerContext.PartitionKeyPath == "/id")
                return;


            object? partitionKey = null;
            if (!string.IsNullOrEmpty(partitionKeyRawValue))
                JsonHelper.TryParseJsonValue(partitionKeyRawValue, out partitionKey);

            var pathParts = containerContext.PartitionKeyPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            JObject current = document;
            // Find or construct the path to the partition key property
            for (int i = 0; i < pathParts.Length; i++)
            {
                var part = pathParts[i];
                var token = current.GetValue(part);
                if (i == pathParts.Length - 1)
                {
                    current[part] = partitionKey is null
                        ? JValue.CreateNull()
                        : JValue.FromObject(partitionKey);
                    return;
                }
                else
                {
                    if (token is JObject obj)
                    {
                        current = obj;
                    }
                    else if (token is null)
                    {
                        obj = new JObject();
                        current[part] = obj;
                        current = obj;
                    }
                    else
                    {
                        // A property exists, but its value is not an object. Nothing
                        // we can do here, give up. Shouldn't happen anyway, since we
                        // control how the document is constructed.
                        return;
                    }
                }
            }
        }

        private QueryParameterViewModel CreateParameter(QuerySheetParameter p)
        {
            var pvm = new QueryParameterViewModel
            {
                Name = p.Name,
                RawValue = p.RawValue,
            };
            foreach (var mru in p.MRU)
            {
                pvm.MRU.Add(mru);
            }
            return pvm;
        }

        private DelegateCommand? _changeContainerCommand;
        public ICommand ChangeContainerCommand => _changeContainerCommand ??= new DelegateCommand(ChangeContainer);

        private void ChangeContainer()
        {
            var vm = _viewModelFactory.CreateContainerPicker();
            _dialogService.ShowDialog(vm);
            if (vm.SelectedContainer is ContainerNodeViewModel selected)
            {
                SetContainer(selected);
            }
        }

        private void SetContainer(ContainerNodeViewModel container)
        {
            _containerContext = container.Context;
            _executeCommand?.RaiseCanExecuteChanged();
            _newDocumentCommand?.RaiseCanExecuteChanged();
            OnPropertyChanged(null);
        }

        private bool _isQueryRunning;
        public bool IsQueryRunning
        {
            get => _isQueryRunning;
            set => Set(ref _isQueryRunning, value)
                .AndNotifyPropertyChanged(nameof(IsUIEnabled));
        }

        public bool IsUIEnabled => !IsQueryRunning;

        private bool _hasChanges;
        public override bool HasChanges
        {
            get => _hasChanges;
        }

        public void SetHasChanges(bool value) => Set(ref _hasChanges, value, propertyName: nameof(HasChanges));

        private ContainerNodeViewModel? _explorerSelectedContainer;
        public ContainerNodeViewModel? ExplorerSelectedContainer
        {
            get => _explorerSelectedContainer;
            set => Set(ref _explorerSelectedContainer, value)
                .AndNotifyPropertyChanged(nameof(CanSwitchToExplorerSelectedContainer))
                .AndRaiseCanExecuteChanged(_switchToExplorerSelectedContainerCommand);
        }

        private void OnExplorerSelectedContainerChanged(ExplorerSelectedContainerChangedMessage message)
        {
            ExplorerSelectedContainer = message.Container;
        }

        private DelegateCommand? _switchToExplorerSelectedContainerCommand;
        public ICommand SwitchToExplorerSelectedContainerCommand => _switchToExplorerSelectedContainerCommand
            ??= new DelegateCommand(SwitchToExplorerSelectedContainer, () => CanSwitchToExplorerSelectedContainer);

        private void SwitchToExplorerSelectedContainer()
        {
            if (ExplorerSelectedContainer is ContainerNodeViewModel container)
            {
                SetContainer(container);
            }
        }

        public bool CanSwitchToExplorerSelectedContainer =>
            ExplorerSelectedContainer is ContainerNodeViewModel c && c.Path != this.ContainerPath;

        public void Save(string path)
        {
            var querySheet = GetQuerySheet();
            _queryPersistenceService.Save(querySheet, path);
            FilePath = path;
            SetHasChanges(false);
        }

        public string FileFilter => QuerySheet.FileFilter;
    }
}