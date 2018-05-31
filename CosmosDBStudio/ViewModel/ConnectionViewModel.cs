using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class ConnectionViewModel : NonLeafTreeNodeViewModel
    {
        private readonly DatabaseConnection _connection;
        private readonly IConnectionBrowserService _connectionBrowserService;
        private readonly IViewModelFactory _viewModelFactory;

        public ConnectionViewModel(
            DatabaseConnection connection,
            IConnectionBrowserService connectionBrowserService,
            IViewModelFactory viewModelFactory)
        {
            _connection = connection;
            _connectionBrowserService = connectionBrowserService;
            _viewModelFactory = viewModelFactory;
            _name = connection.Name;
        }

        public string Id => _connection.Id;

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value).AndNotifyPropertyChanged(nameof(Text));
        }

        public override string Text => _name;

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var databases = await _connectionBrowserService.GetDatabasesAsync(_connection.Id);
            return databases.Select(id => _viewModelFactory.CreateDatabaseViewModel(this, id)).ToList();
        }
    }
}