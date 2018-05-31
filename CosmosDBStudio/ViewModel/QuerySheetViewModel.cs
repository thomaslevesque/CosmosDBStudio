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
        private readonly IConnectionDirectory _connectionDirectory;
        private readonly QuerySheet _querySheet;

        public QuerySheetViewModel(IQueryExecutionService queryExecutionService, IViewModelFactory viewModelFactory, IConnectionDirectory connectionDirectory, QuerySheet querySheet)
        {
            _queryExecutionService = queryExecutionService;
            _viewModelFactory = viewModelFactory;
            _connectionDirectory = connectionDirectory;
            _querySheet = querySheet;
            _title = string.IsNullOrEmpty(querySheet.Path)
                ? $"Untitled {++_untitledCounter}"
                : Path.GetFileNameWithoutExtension(querySheet.Path);
            _text = querySheet.Text;
            _connectionId = querySheet.ConnectionId;
            _databaseId = querySheet.DatabaseId;
            _collectionId = querySheet.CollectionId;

            _executeCommand = new AsyncDelegateCommand(ExecuteAsync, CanExecute);
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

        private string _connectionId;

        public string ConnectionId
        {
            get => _connectionId;
            set => Set(ref _connectionId, value);
        }

        private string _databaseId;

        public string DatabaseId
        {
            get => _databaseId;
            set => Set(ref _databaseId, value);
        }

        private string _collectionId;

        public string CollectionId
        {
            get => _collectionId;
            set => Set(ref _collectionId, value);
        }

        public string CurrentConnectionPath => $"{_connectionDirectory.GetById(ConnectionId)?.Name ?? "??"}/{DatabaseId}/{CollectionId}";

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
                ConnectionId = ConnectionId,
                DatabaseId = DatabaseId,
                CollectionId = CollectionId,
                Sql = SelectedText
            };
            var result = await _queryExecutionService.ExecuteAsync(query);
            Result = _viewModelFactory.CreateQueryResultViewModel(result);
        }
    }
}