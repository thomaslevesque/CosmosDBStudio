using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        QuerySheetViewModel CreateQuerySheet(QuerySheet querySheet, string? path, IContainerContext? containerContext);
        QueryResultViewModel CreateQueryResult(QueryResult result, IContainerContext containerContext);
        AccountViewModel CreateAccountNode(CosmosAccount account, AccountFolderViewModel? parent);
        AccountFolderViewModel CreateAccountFolderNode(CosmosAccountFolder folder, AccountFolderViewModel? parent);
        DatabaseViewModel CreateDatabaseNode(AccountViewModel account, string id);
        ContainerViewModel CreateContainerNode(DatabaseViewModel database, string id);
        AccountsViewModel CreateAccounts();
        NotRunQueryResultViewModel CreateNotRunQueryResult();
        ResultItemViewModel CreateDocument(JToken document, IContainerContext containerContext);
        ResultItemViewModel CreateErrorItemPlaceholder();
        ResultItemViewModel CreateEmptyResultPlaceholder();
        DocumentEditorViewModel CreateDocumentEditor(JObject document, bool isNew, IContainerContext containerContext);
        AccountEditorViewModel CreateAccountEditor(CosmosAccount? account = null);
        ContainerPickerViewModel CreateContainerPicker();
        DatabaseEditorViewModel CreateDatabaseEditor(CosmosDatabase? database = null);
        ContainerEditorViewModel CreateContainerEditor(CosmosContainer? container, bool databaseHasProvisionedThroughput);
        StoredProceduresFolderViewModel CreateStoredProceduresFolderNode(ContainerViewModel container);
        StoredProcedureViewModel CreateStoredProcedureNode(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosStoredProcedure storedProcedure);
        UserDefinedFunctionsFolderViewModel CreateUserDefinedFunctionsFolderNode(ContainerViewModel container);
        UserDefinedFunctionViewModel CreateUserDefinedFunctionNode(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosUserDefinedFunction udf);
        TriggersFolderViewModel CreateTriggersFolderNode(ContainerViewModel container);
        TriggerViewModel CreateTriggerNode(ContainerViewModel container, NonLeafTreeNodeViewModel parent, CosmosTrigger trigger);
        StoredProcedureEditorViewModel CreateStoredProcedureEditor(CosmosStoredProcedure storedProcedure, IContainerContext containerContext);
        UserDefinedFunctionEditorViewModel CreateUserDefinedFunctionEditor(CosmosUserDefinedFunction udf, IContainerContext containerContext);
        TriggerEditorViewModel CreateTriggerEditor(CosmosTrigger trigger, IContainerContext containerContext);
    }
}
