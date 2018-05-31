using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IConnectionBrowserService _connectionBrowserService;
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseViewModel(ConnectionViewModel connection, string id, IConnectionBrowserService connectionBrowserService, IViewModelFactory viewModelFactory)
        {
            _connectionBrowserService = connectionBrowserService;
            _viewModelFactory = viewModelFactory;
            Connection = connection;
            Id = id;
        }

        public ConnectionViewModel Connection { get; }

        public string Id { get; }

        public override string Text => Id;

        protected override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            return Task.FromResult<IEnumerable<TreeNodeViewModel>>(
                new[]
                {
                    _viewModelFactory.CreateCollectionsNodeViewModel(this)
                });
        }
    }
}