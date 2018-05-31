using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class CollectionsNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IConnectionBrowserService _connectionBrowserService;
        private readonly IViewModelFactory _viewModelFactory;
        public DatabaseViewModel Database { get; }

        public CollectionsNodeViewModel(DatabaseViewModel database, IConnectionBrowserService connectionBrowserService, IViewModelFactory viewModelFactory)
        {
            _connectionBrowserService = connectionBrowserService;
            _viewModelFactory = viewModelFactory;
            Database = database;
        }

        public override string Text => "Collections";

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var collections = await _connectionBrowserService.GetCollectionsAsync(Database.Connection.Id, Database.Id);
            return collections.Select(id => _viewModelFactory.CreateCollectionViewModel(Database, id)).ToList();
        }
    }
}
