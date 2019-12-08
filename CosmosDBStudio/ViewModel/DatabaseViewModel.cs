using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IAccountBrowserService _accountBrowserService;
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseViewModel(AccountViewModel account, string id, IAccountBrowserService accountBrowserService, IViewModelFactory viewModelFactory)
        {
            _accountBrowserService = accountBrowserService;
            _viewModelFactory = viewModelFactory;
            Account = account;
            Id = id;
        }

        public AccountViewModel Account { get; }

        public string Id { get; }

        public override string Text => Id;

        public override NonLeafTreeNodeViewModel? Parent => Account;

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var containers = await _accountBrowserService.GetContainersAsync(Account.Id, Id);
            return containers.Select(id => _viewModelFactory.CreateContainerViewModel(this, id)).ToList();
        }
    }
}