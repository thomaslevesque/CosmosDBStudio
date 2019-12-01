using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class AccountViewModel : NonLeafTreeNodeViewModel
    {
        private readonly CosmosAccount _account;
        private readonly IAccountBrowserService _accountBrowserService;
        private readonly IViewModelFactory _viewModelFactory;

        public AccountViewModel(
            CosmosAccount account,
            IAccountBrowserService accountBrowserService,
            IViewModelFactory viewModelFactory)
        {
            _account = account;
            _accountBrowserService = accountBrowserService;
            _viewModelFactory = viewModelFactory;
            _name = account.Name;
        }

        public string Id => _account.Id;

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value).AndNotifyPropertyChanged(nameof(Text));
        }

        public override string Text => _name;

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var databases = await _accountBrowserService.GetDatabasesAsync(_account.Id);
            return databases.Select(id => _viewModelFactory.CreateDatabaseViewModel(this, id)).ToList();
        }
    }
}