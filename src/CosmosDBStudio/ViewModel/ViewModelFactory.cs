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
        private readonly ScriptCommands<CosmosStoredProcedure> _storedProcedureCommands;
        private readonly ScriptCommands<CosmosUserDefinedFunction> _udfCommands;
        private readonly ScriptCommands<CosmosTrigger> _triggerCommands;
        private readonly IMessenger _messenger;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IContainerContextFactory _containerContextFactory;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IDialogService _dialogService;
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IClipboardService _clipboardService;
        private readonly IQueryPersistenceService _queryPersistenceService;

        public ViewModelFactory(
            AccountCommands accountCommands,
            DatabaseCommands databaseCommands,
            ContainerCommands containerCommands,
            ScriptCommands<CosmosStoredProcedure> storedProcedureCommands,
            ScriptCommands<CosmosUserDefinedFunction> udfCommands,
            ScriptCommands<CosmosTrigger> triggerCommands,
            IMessenger messenger,
            IAccountDirectory accountDirectory,
            IContainerContextFactory containerContextFactory,
            ICosmosAccountManager accountManager,
            IDialogService dialogService,
            IUIDispatcher uiDispatcher,
            IClipboardService clipboardService,
            IQueryPersistenceService queryPersistenceService)
        {
            _accountCommands = accountCommands;
            _databaseCommands = databaseCommands;
            _containerCommands = containerCommands;
            _storedProcedureCommands = storedProcedureCommands;
            _udfCommands = udfCommands;
            _triggerCommands = triggerCommands;
            _messenger = messenger;
            _accountDirectory = accountDirectory;
            _containerContextFactory = containerContextFactory;
            _accountManager = accountManager;
            _dialogService = dialogService;
            _uiDispatcher = uiDispatcher;
            _clipboardService = clipboardService;
            _queryPersistenceService = queryPersistenceService;
        }

        public QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet, string? path, IContainerContext? containerContext)
        {
            return new QuerySheetViewModel(this, _dialogService, _containerContextFactory, _messenger, _queryPersistenceService, querySheet, path, containerContext);
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
            return new ContainerViewModel(database, id, _containerCommands, this);
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

        public StoredProceduresFolderViewModel CreateStoredProceduresFolder(ContainerViewModel container)
        {
            return new StoredProceduresFolderViewModel(container, _accountManager, _storedProcedureCommands, this);
        }

        public StoredProcedureViewModel CreateStoredProcedure(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosStoredProcedure storedProcedure)
        {
            return new StoredProcedureViewModel(container, parent, storedProcedure, _storedProcedureCommands, _messenger);
        }

        public StoredProcedureEditorViewModel CreateStoredProcedureEditor(CosmosStoredProcedure storedProcedure, IContainerContext containerContext)
        {
            return new StoredProcedureEditorViewModel(containerContext, storedProcedure);
        }

        public UserDefinedFunctionsFolderViewModel CreateUserDefinedFunctionsFolder(ContainerViewModel container)
        {
            return new UserDefinedFunctionsFolderViewModel(container, _accountManager, _udfCommands, this);
        }

        public UserDefinedFunctionViewModel CreateUserDefinedFunction(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosUserDefinedFunction udf)
        {
            return new UserDefinedFunctionViewModel(container, parent, udf, _udfCommands, _messenger);
        }

        public UserDefinedFunctionEditorViewModel CreateUserDefinedFunctionEditor(CosmosUserDefinedFunction udf, IContainerContext containerContext)
        {
            return new UserDefinedFunctionEditorViewModel(containerContext, udf);
        }

        public TriggersFolderViewModel CreateTriggersFolder(ContainerViewModel container)
        {
            return new TriggersFolderViewModel(container, _accountManager, _triggerCommands, this);
        }

        public TriggerViewModel CreateTrigger(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosTrigger trigger)
        {
            return new TriggerViewModel(container, parent, trigger, _triggerCommands, _messenger);
        }

        public TriggerEditorViewModel CreateTriggerEditor(CosmosTrigger trigger, IContainerContext containerContext)
        {
            return new TriggerEditorViewModel(containerContext, trigger);
        }
    }
}