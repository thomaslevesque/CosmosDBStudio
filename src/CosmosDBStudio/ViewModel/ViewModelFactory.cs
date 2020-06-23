using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly AccountCommands _accountCommands;
        private readonly DatabaseCommands _databaseCommands;
        private readonly ContainerCommands _containerCommands;
        private readonly IMessenger _messenger;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IContainerContextFactory _containerContextFactory;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IDialogService _dialogService;
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IClipboardService _clipboardService;

        public ViewModelFactory(
            AccountCommands accountCommands,
            DatabaseCommands databaseCommands,
            ContainerCommands containerCommands,
            IMessenger messenger,
            IAccountDirectory accountDirectory,
            IContainerContextFactory containerContextFactory,
            ICosmosAccountManager accountManager,
            IDialogService dialogService,
            IUIDispatcher uiDispatcher,
            IClipboardService clipboardService)
        {
            _accountCommands = accountCommands;
            _databaseCommands = databaseCommands;
            _containerCommands = containerCommands;
            _messenger = messenger;
            _accountDirectory = accountDirectory;
            _containerContextFactory = containerContextFactory;
            _accountManager = accountManager;
            _dialogService = dialogService;
            _uiDispatcher = uiDispatcher;
            _clipboardService = clipboardService;
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet, string? path, IContainerContext? containerContext)
        {
            return new QuerySheetViewModel(this, _dialogService, _containerContextFactory, _messenger, querySheet, path, containerContext);
        }

        public NotRunQueryResultViewModel CreateNotRunQueryResultViewModel()
        {
            return new NotRunQueryResultViewModel();
        }

        public QueryResultViewModel CreateQueryResultViewModel(QueryResult result, IContainerContext containerContext)
        {
            return new QueryResultViewModel(result, containerContext, this, _dialogService, _messenger);
        }

        public AccountsViewModel CreateAccountsViewModel()
        {
            return new AccountsViewModel(_accountCommands, this, _accountDirectory, _messenger);
        }

        public AccountViewModel CreateAccountViewModel(CosmosAccount account, AccountFolderViewModel? parent)
        {
            return new AccountViewModel(account, parent, _accountCommands, _databaseCommands, _accountManager, this, _messenger);
        }

        public AccountFolderViewModel CreateAccountFolderViewModel(CosmosAccountFolder folder, AccountFolderViewModel? parent)
        {
            return new AccountFolderViewModel(folder, parent, _accountCommands, _accountDirectory, this);
        }

        public DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id)
        {
            return new DatabaseViewModel(account, id, _databaseCommands, _containerCommands, _accountManager, this, _messenger);
        }

        public ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id)
        {
            return new ContainerViewModel(database, id, _containerCommands);
        }

        public ResultItemViewModel CreateDocumentViewModel(JToken document, IContainerContext containerContext)
        {
            return new DocumentViewModel(document, containerContext);
        }

        public ResultItemViewModel CreateErrorItemPlaceholder()
        {
            return new ErrorItemPlaceholderViewModel();
        }

        public ResultItemViewModel CreateEmptyResultPlaceholder()
        {
            return new EmptyResultItemPlaceholderViewModel();
        }

        public DocumentEditorViewModel CreateDocumentEditorViewModel(
            JObject document,
            bool isNew,
            IContainerContext containerContext)
        {
            return new DocumentEditorViewModel(document, isNew, containerContext, _dialogService, _uiDispatcher);
        }

        public AccountEditorViewModel CreateAccountEditorViewModel(CosmosAccount? account = null)
        {
            return new AccountEditorViewModel(account, _clipboardService);
        }

        public DatabaseEditorViewModel CreateDatabaseEditorViewModel(CosmosDatabase? database = null)
        {
            return new DatabaseEditorViewModel(database);
        }

        public ContainerPickerViewModel CreateContainerPickerViewModel()
        {
            return new ContainerPickerViewModel(this, _accountDirectory);
        }

        public ContainerEditorViewModel CreateContainerEditorViewModel(bool databaseHasProvisionedThroughput, CosmosContainer? container = null)
        {
            return new ContainerEditorViewModel(container, databaseHasProvisionedThroughput);
        }
    }
}