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
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IClipboardService _clipboardService;

        public ViewModelFactory(
            IMessenger messenger,
            IAccountDirectory accountDirectory,
            IContainerContextFactory containerContextFactory,
            IAccountBrowserService accountBrowserService,
            IDialogService dialogService,
            IUIDispatcher uiDispatcher,
            IClipboardService clipboardService)
        {
            _messenger = messenger;
            _accountDirectory = accountDirectory;
            _containerContextFactory = containerContextFactory;
            _accountBrowserService = accountBrowserService;
            _dialogService = dialogService;
            _uiDispatcher = uiDispatcher;
            _clipboardService = clipboardService;
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet, string? path, IContainerContext? containerContext)
        {
            return new QuerySheetViewModel(this, _dialogService, _containerContextFactory, querySheet, path, containerContext);
        }

        public NotRunQueryResultViewModel CreateNotRunQueryResultViewModel()
        {
            return new NotRunQueryResultViewModel();
        }

        public QueryResultViewModel CreateQueryResultViewModel(QueryResult result, IContainerContext containerContext)
        {
            return new QueryResultViewModel(result, containerContext, this, _dialogService);
        }

        public AccountsViewModel CreateAccountsViewModel()
        {
            return new AccountsViewModel(this, _accountDirectory, _dialogService);
        }

        public AccountViewModel CreateAccountViewModel(CosmosAccount account, AccountFolderViewModel? parent)
        {
            return new AccountViewModel(account, parent, _accountBrowserService, this);
        }

        public AccountFolderViewModel CreateAccountFolderViewModel(CosmosAccountFolder folder, AccountFolderViewModel? parent)
        {
            return new AccountFolderViewModel(folder, parent, _accountDirectory, this);
        }

        public DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id)
        {
            return new DatabaseViewModel(account, id, _accountBrowserService, this);
        }

        public ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id)
        {
            return new ContainerViewModel(database, id, _messenger);
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

        public ContainerPickerViewModel CreateContainerPickerViewModel()
        {
            return new ContainerPickerViewModel(this, _accountDirectory);
        }
    }
}