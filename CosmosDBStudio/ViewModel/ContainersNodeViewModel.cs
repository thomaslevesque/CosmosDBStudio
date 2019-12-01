using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class ContainersNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IAccountBrowserService _accountBrowserService;
        private readonly IViewModelFactory _viewModelFactory;
        public DatabaseViewModel Database { get; }

        public ContainersNodeViewModel(DatabaseViewModel database, IAccountBrowserService accountBrowserService, IViewModelFactory viewModelFactory)
        {
            _accountBrowserService = accountBrowserService;
            _viewModelFactory = viewModelFactory;
            Database = database;
        }

        public override string Text => "Containers";

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var containers = await _accountBrowserService.GetContainersAsync(Database.Account.Id, Database.Id);
            return containers.Select(id => _viewModelFactory.CreateContainerViewModel(Database, id)).ToList();
        }
    }
}
