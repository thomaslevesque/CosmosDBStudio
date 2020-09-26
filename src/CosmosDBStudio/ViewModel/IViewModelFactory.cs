using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        QuerySheetViewModel CreateQuerySheet(QuerySheet querySheet, string? path, IContainerContext? containerContext);
        QueryResultViewModel CreateQueryResult(QueryResult result, IContainerContext containerContext);
        AccountNodeViewModel CreateAccountNode(CosmosAccount account, AccountFolderNodeViewModel? parent);
        AccountFolderNodeViewModel CreateAccountFolderNode(CosmosAccountFolder folder, AccountFolderNodeViewModel? parent);
        DatabaseNodeViewModel CreateDatabaseNode(AccountNodeViewModel account, string id);
        ContainerNodeViewModel CreateContainerNode(DatabaseNodeViewModel database, string id);
        AccountsViewModel CreateAccounts();
        NotRunQueryResultViewModel CreateNotRunQueryResult();
        DocumentResultViewModel CreateDocumentResult(JToken document, IContainerContext containerContext);
        ErrorItemPlaceholderViewModel CreateErrorItemPlaceholder();
        EmptyResultItemPlaceholderViewModel CreateEmptyResultPlaceholder();
        DocumentEditorViewModel CreateDocumentEditor(JObject document, bool isNew, IContainerContext containerContext);
        AccountEditorViewModel CreateAccountEditor(CosmosAccount? account = null);
        ContainerPickerViewModel CreateContainerPicker();
        DatabaseEditorViewModel CreateDatabaseEditor(CosmosDatabase? database = null);
        ContainerEditorViewModel CreateContainerEditor(CosmosContainer? container, bool databaseHasProvisionedThroughput);
        StoredProceduresFolderNodeViewModel CreateStoredProceduresFolderNode(ContainerNodeViewModel container);
        StoredProcedureNodeViewModel CreateStoredProcedureNode(ContainerNodeViewModel container, NonLeafTreeNodeViewModel parent, CosmosStoredProcedure storedProcedure);
        UserDefinedFunctionsFolderNodeViewModel CreateUserDefinedFunctionsFolderNode(ContainerNodeViewModel container);
        UserDefinedFunctionNodeViewModel CreateUserDefinedFunctionNode(ContainerNodeViewModel container, NonLeafTreeNodeViewModel parent, CosmosUserDefinedFunction udf);
        TriggersFolderNodeViewModel CreateTriggersFolderNode(ContainerNodeViewModel container);
        TriggerNodeViewModel CreateTriggerNode(ContainerNodeViewModel container, NonLeafTreeNodeViewModel parent, CosmosTrigger trigger);
        StoredProcedureEditorViewModel CreateStoredProcedureEditor(CosmosStoredProcedure storedProcedure, IContainerContext containerContext);
        UserDefinedFunctionEditorViewModel CreateUserDefinedFunctionEditor(CosmosUserDefinedFunction udf, IContainerContext containerContext);
        TriggerEditorViewModel CreateTriggerEditor(CosmosTrigger trigger, IContainerContext containerContext);
    }
}
