using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class QuerySheetViewModel : BindableBase
    {
        private static int _untitledCounter;

        private readonly IQueryExecutionService _queryExecutionService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IAccountDirectory _accountDirectory;
        private readonly QuerySheet _querySheet;

        public QuerySheetViewModel(IQueryExecutionService queryExecutionService, IViewModelFactory viewModelFactory, IAccountDirectory accountDirectory, QuerySheet querySheet)
        {
            _queryExecutionService = queryExecutionService;
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            _querySheet = querySheet;
            _title = string.IsNullOrEmpty(querySheet.Path)
                ? $"Untitled {++_untitledCounter}"
                : Path.GetFileNameWithoutExtension(querySheet.Path);
            _text = querySheet.Text;
            _accountId = querySheet.AccountId;
            _databaseId = querySheet.DatabaseId;
            _containerId = querySheet.ContainerId;

            _executeCommand = new AsyncDelegateCommand(ExecuteAsync, CanExecute);
            _closeCommand = new DelegateCommand(Close);

            Result = _viewModelFactory.CreateEmptyResultViewModel();
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndExecute(_executeCommand.RaiseCanExecuteChanged);
        }

        private string _accountId;

        public string AccountId
        {
            get => _accountId;
            set => Set(ref _accountId, value);
        }

        private string _databaseId;

        public string DatabaseId
        {
            get => _databaseId;
            set => Set(ref _databaseId, value);
        }

        private string _containerId;

        public string ContainerId
        {
            get => _containerId;
            set => Set(ref _containerId, value);
        }

        public string ContainerPath => $"{_accountDirectory.GetById(AccountId)?.Name ?? "??"}/{DatabaseId}/{ContainerId}";

        private string _selectedText;
        public string SelectedText
        {
            get => _selectedText;
            set => Set(ref _selectedText, value)
                .AndExecute(_executeCommand.RaiseCanExecuteChanged);
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
                .AndExecute(_executeCommand.RaiseCanExecuteChanged);
        }

        private QueryResultViewModel _result;

        public QueryResultViewModel Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        private readonly AsyncDelegateCommand _executeCommand;
        public ICommand ExecuteCommand => _executeCommand;

        private bool CanExecute()
        {
            return !string.IsNullOrEmpty(SelectedText)
                || !string.IsNullOrEmpty(ExtendSelectionAroundCursor(false));
        }

        private async Task ExecuteAsync()
        {
            var queryText = SelectedText;
            if (string.IsNullOrEmpty(queryText))
                queryText = ExtendSelectionAroundCursor(true);

            if (string.IsNullOrEmpty(queryText))
                return;

            // TODO: parameters, options
            var query = new Query
            {
                AccountId = AccountId,
                DatabaseId = DatabaseId,
                ContainerId = ContainerId,
                Sql = queryText
            };
            var result = await _queryExecutionService.ExecuteAsync(query);
            Result = _viewModelFactory.CreateQueryResultViewModel(result);
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

        private readonly DelegateCommand _closeCommand;
        public ICommand CloseCommand => _closeCommand;

        private void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseRequested;
    }
}