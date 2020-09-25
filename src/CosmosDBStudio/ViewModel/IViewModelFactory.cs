using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet, string? path, IContainerContext? containerContext);
        QueryResultViewModel CreateQueryResultViewModel(QueryResult result, IContainerContext containerContext);
        AccountViewModel CreateAccountViewModel(CosmosAccount account, AccountFolderViewModel? parent);
        AccountFolderViewModel CreateAccountFolderViewModel(CosmosAccountFolder folder, AccountFolderViewModel? parent);
        DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id);
        ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id);
        AccountsViewModel CreateAccountsViewModel();
        NotRunQueryResultViewModel CreateNotRunQueryResultViewModel();
        ResultItemViewModel CreateDocumentViewModel(JToken document, IContainerContext containerContext);
        ResultItemViewModel CreateErrorItemPlaceholder();
        ResultItemViewModel CreateEmptyResultPlaceholder();
        DocumentEditorViewModel CreateDocumentEditorViewModel(
            JObject document,
            bool isNew,
            IContainerContext containerContext);
        AccountEditorViewModel CreateAccountEditorViewModel(CosmosAccount? account = null);
        ContainerPickerViewModel CreateContainerPickerViewModel();
        DatabaseEditorViewModel CreateDatabaseEditorViewModel(CosmosDatabase? database = null);
        ContainerEditorViewModel CreateContainerEditorViewModel(bool databaseHasProvisionedThroughput, CosmosContainer? container = null);
        StoredProceduresFolderViewModel CreateStoredProceduresFolder(ContainerViewModel container);
        StoredProcedureViewModel CreateStoredProcedure(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosStoredProcedure storedProcedure);
        UserDefinedFunctionsFolderViewModel CreateUserDefinedFunctionsFolder(ContainerViewModel container);
        UserDefinedFunctionViewModel CreateUserDefinedFunction(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosUserDefinedFunction udf);
        TriggersFolderViewModel CreateTriggersFolder(ContainerViewModel container);
        TriggerViewModel CreateTrigger(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosTrigger trigger);
        StoredProcedureEditorViewModel CreateStoredProcedureEditor(CosmosStoredProcedure storedProcedure, IContainerContext containerContext);
        UserDefinedFunctionEditorViewModel CreateUserDefinedFunctionEditor(CosmosUserDefinedFunction udf, IContainerContext containerContext);
        TriggerEditorViewModel CreateTriggerEditor(CosmosTrigger trigger, IContainerContext containerContext);
    }
}
