using System.Collections.ObjectModel;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class ConnectionsViewModel : BindableBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IConnectionDirectory _connectionDirectory;

        public ConnectionsViewModel(IViewModelFactory viewModelFactory, IConnectionDirectory connectionDirectory)
        {
            _viewModelFactory = viewModelFactory;
            _connectionDirectory = connectionDirectory;
            Connections = new ObservableCollection<ConnectionViewModel>();
            LoadConnections();
        }

        private void LoadConnections()
        {
            foreach (var connection in _connectionDirectory.Connections)
            {
                Connections.Add(_viewModelFactory.CreateConnectionViewModel(connection));
            }
        }

        public ObservableCollection<ConnectionViewModel> Connections { get; }
    }
}
