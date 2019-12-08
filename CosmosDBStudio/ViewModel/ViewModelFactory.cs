using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IMessenger _messenger;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IContainerContextFactory _containerContextFactory;
        private readonly IAccountBrowserService _accountBrowserService;
        private readonly IDialogService _dialogService;

        public ViewModelFactory(
            IMessenger messenger,
            IAccountDirectory accountDirectory,
            IContainerContextFactory containerContextFactory,
            IAccountBrowserService accountBrowserService,
            IDialogService dialogService)
        {
            _messenger = messenger;
            _accountDirectory = accountDirectory;
            _containerContextFactory = containerContextFactory;
            _accountBrowserService = accountBrowserService;
            _dialogService = dialogService;
        }

        public MainWindowViewModel CreateMainWindowViewModel()
        {
            return new MainWindowViewModel(this, _containerContextFactory, _messenger);
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(IContainerContext containerContext, QuerySheet querySheet)
        {
            return new QuerySheetViewModel(containerContext, this, querySheet);
        }

        public NotRunQueryResultViewModel CreateNotRunQueryResultViewModel()
        {
            return new NotRunQueryResultViewModel();
        }

        public QueryResultViewModel CreateQueryResultViewModel(QueryResult result)
        {
            return new QueryResultViewModel(result, this);
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

        public ResultItemViewModel CreateDocumentViewModel(JToken document)
        {
            return new DocumentViewModel(document);
        }

        public ResultItemViewModel CreateErrorItemPlaceholder()
        {
            return new ErrorItemPlaceholderViewModel();
        }

        public ResultItemViewModel CreateEmptyResultPlaceholder()
        {
            return new EmptyResultItemPlaceholderViewModel();
        }
    }
}