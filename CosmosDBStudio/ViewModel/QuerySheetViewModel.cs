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
            set => Set(ref _text, value);
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

        private QueryResultViewModel _result;

        public QueryResultViewModel Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        private readonly AsyncDelegateCommand _executeCommand;
        public ICommand ExecuteCommand => _executeCommand;

        private bool CanExecute() => !string.IsNullOrEmpty(SelectedText);

        private async Task ExecuteAsync()
        {
            // TODO: parameters, options
            var query = new Query
            {
                AccountId = AccountId,
                DatabaseId = DatabaseId,
                ContainerId = ContainerId,
                Sql = SelectedText
            };
            var result = await _queryExecutionService.ExecuteAsync(query);
            Result = _viewModelFactory.CreateQueryResultViewModel(result);
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