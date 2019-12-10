using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        MainWindowViewModel CreateMainWindowViewModel();
        QuerySheetViewModel CreateQuerySheetViewModel(IContainerContext containerContext, QuerySheet querySheet);
        QueryResultViewModel CreateQueryResultViewModel(QueryResult result, IContainerContext containerContext);
        AccountViewModel CreateAccountViewModel(CosmosAccount account);
        DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id);
        ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id);
        AccountsViewModel CreateAccountsViewModel();
        NotRunQueryResultViewModel CreateNotRunQueryResultViewModel();
        ResultItemViewModel CreateDocumentViewModel(JToken document, IContainerContext containerContext);
        ResultItemViewModel CreateErrorItemPlaceholder();
        ResultItemViewModel CreateEmptyResultPlaceholder();
    }
}
