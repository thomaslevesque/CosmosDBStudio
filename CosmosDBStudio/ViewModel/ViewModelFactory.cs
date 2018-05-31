using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IMessenger _messenger;
        private readonly IQueryExecutionService _queryExecutionService;
        private readonly IConnectionDirectory _connectionDirectory;
        private readonly IConnectionBrowserService _connectionBrowserService;

        public ViewModelFactory(
            IMessenger messenger,
            IQueryExecutionService queryExecutionService,
            IConnectionDirectory connectionDirectory,
            IConnectionBrowserService connectionBrowserService)
        {
            _messenger = messenger;
            _queryExecutionService = queryExecutionService;
            _connectionDirectory = connectionDirectory;
            _connectionBrowserService = connectionBrowserService;
        }

        public MainWindowViewModel CreateMainWindowViewModel()
        {
            return new MainWindowViewModel(this, _messenger);
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet)
        {
            return new QuerySheetViewModel(_queryExecutionService, this, querySheet);
        }

        public QueryResultViewModel CreateQueryResultViewModel(QueryResult result)
        {
            return new QueryResultViewModel(result);
        }

        public ConnectionsViewModel CreateConnectionsViewModel()
        {
            return new ConnectionsViewModel(this, _connectionDirectory);
        }

        public ConnectionViewModel CreateConnectionViewModel(DatabaseConnection connection)
        {
            return new ConnectionViewModel(connection, _connectionBrowserService, this);
        }

        public DatabaseViewModel CreateDatabaseViewModel(ConnectionViewModel connection, string id)
        {
            return new DatabaseViewModel(connection, id, _connectionBrowserService, this);
        }

        public CollectionsNodeViewModel CreateCollectionsNodeViewModel(DatabaseViewModel database)
        {
            return new CollectionsNodeViewModel(database, _connectionBrowserService, this);
        }

        public CollectionViewModel CreateCollectionViewModel(DatabaseViewModel database, string id)
        {
            return new CollectionViewModel(database, id, _messenger);
        }
    }
}