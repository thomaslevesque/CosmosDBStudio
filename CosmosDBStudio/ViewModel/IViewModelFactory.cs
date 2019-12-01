using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        MainWindowViewModel CreateMainWindowViewModel();
        QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet);
        QueryResultViewModel CreateQueryResultViewModel(QueryResult result);
        ConnectionViewModel CreateConnectionViewModel(DatabaseConnection connection);
        DatabaseViewModel CreateDatabaseViewModel(ConnectionViewModel connection, string id);
        ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id);
        ConnectionsViewModel CreateConnectionsViewModel();
    }
}
