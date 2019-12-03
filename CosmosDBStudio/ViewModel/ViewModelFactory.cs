using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IMessenger _messenger;
        private readonly IQueryExecutionService _queryExecutionService;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IAccountBrowserService _accountBrowserService;
        private readonly IDialogService _dialogService;

        public ViewModelFactory(
            IMessenger messenger,
            IQueryExecutionService queryExecutionService,
            IAccountDirectory accountDirectory,
            IAccountBrowserService accountBrowserService,
            IDialogService dialogService)
        {
            _messenger = messenger;
            _queryExecutionService = queryExecutionService;
            _accountDirectory = accountDirectory;
            _accountBrowserService = accountBrowserService;
            _dialogService = dialogService;
        }

        public MainWindowViewModel CreateMainWindowViewModel()
        {
            return new MainWindowViewModel(this, _messenger);
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet)
        {
            return new QuerySheetViewModel(_queryExecutionService, this, _accountDirectory, querySheet);
        }

        public QueryResultViewModel CreateQueryResultViewModel(QueryResult result)
        {
            return new QueryResultViewModel(result);
        }

        public AccountsViewModel CreateAccountsViewModel()
        {
            return new AccountsViewModel(this, _accountDirectory, _dialogService);
        }

        public AccountViewModel CreateAccountViewModel(CosmosAccount account)
        {
            return new AccountViewModel(account, _accountBrowserService, this);
        }

        public DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id)
        {
            return new DatabaseViewModel(account, id, _accountBrowserService, this);
        }

        public ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id)
        {
            return new ContainerViewModel(database, id, _messenger);
        }
    }
}