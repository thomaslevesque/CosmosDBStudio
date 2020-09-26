using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        QuerySheetViewModel CreateQuerySheet(QuerySheet querySheet, string? path, IContainerContext? context);
        QueryResultViewModel CreateQueryResult(QueryResult result, IContainerContext context);
        AccountNodeViewModel CreateAccountNode(CosmosAccount account, IAccountContext accountContext, AccountFolderNodeViewModel? parent);
        AccountFolderNodeViewModel CreateAccountFolderNode(CosmosAccountFolder folder, AccountFolderNodeViewModel? parent);
        DatabaseNodeViewModel CreateDatabaseNode(AccountNodeViewModel account, string id, IDatabaseContext context);
        ContainerNodeViewModel CreateContainerNode(DatabaseNodeViewModel database, string id, IContainerContext context);
        AccountsViewModel CreateAccounts();
        NotRunQueryResultViewModel CreateNotRunQueryResult();
        DocumentResultViewModel CreateDocumentResult(JToken document, IContainerContext context);
        ErrorItemPlaceholderViewModel CreateErrorItemPlaceholder();
        EmptyResultItemPlaceholderViewModel CreateEmptyResultPlaceholder();
        DocumentEditorViewModel CreateDocumentEditor(JObject document, bool isNew, IContainerContext context);
        AccountEditorViewModel CreateAccountEditor(CosmosAccount? account = null);
        ContainerPickerViewModel CreateContainerPicker();
        DatabaseEditorViewModel CreateDatabaseEditor(CosmosDatabase? database = null);
        ContainerEditorViewModel CreateContainerEditor(CosmosContainer? container, bool databaseHasProvisionedThroughput);
        StoredProceduresFolderNodeViewModel CreateStoredProceduresFolderNode(IContainerContext context, NonLeafTreeNodeViewModel parent);
        StoredProcedureNodeViewModel CreateStoredProcedureNode(CosmosStoredProcedure storedProcedure, IContainerContext context, NonLeafTreeNodeViewModel parent);
        UserDefinedFunctionsFolderNodeViewModel CreateUserDefinedFunctionsFolderNode(IContainerContext context, NonLeafTreeNodeViewModel parent);
        UserDefinedFunctionNodeViewModel CreateUserDefinedFunctionNode(CosmosUserDefinedFunction udf, IContainerContext context, NonLeafTreeNodeViewModel parent);
        TriggersFolderNodeViewModel CreateTriggersFolderNode(IContainerContext context, NonLeafTreeNodeViewModel parent);
        TriggerNodeViewModel CreateTriggerNode(CosmosTrigger trigger, IContainerContext context, NonLeafTreeNodeViewModel parent);
        StoredProcedureEditorViewModel CreateStoredProcedureEditor(CosmosStoredProcedure storedProcedure, IContainerContext context);
        UserDefinedFunctionEditorViewModel CreateUserDefinedFunctionEditor(CosmosUserDefinedFunction udf, IContainerContext context);
        TriggerEditorViewModel CreateTriggerEditor(CosmosTrigger trigger, IContainerContext context);
    }
}
