using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseViewModel : NonLeafTreeNodeViewModel
    {
        private readonly ICosmosAccountManager _accountManager;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IDialogService _dialogService;

        public DatabaseViewModel(
            AccountViewModel account,
            string id,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory,
            IDialogService dialogService)
        {
            _accountManager = accountManager;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            Account = account;
            Id = id;
            MenuCommands = new[]
            {
                new MenuCommandViewModel(
                    "Edit database",
                    new AsyncDelegateCommand(EditDatabaseAsync))
            };
        }

        public AccountViewModel Account { get; }

        public string Id { get; }

        public override string Text => Id;

        public override NonLeafTreeNodeViewModel? Parent => Account;

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var containers = await _accountManager.GetContainersAsync(Account.Id, Id);
            return containers.Select(id => _viewModelFactory.CreateContainerViewModel(this, id)).ToList();
        }

        public override IEnumerable<MenuCommandViewModel> MenuCommands { get; }

        private async Task EditDatabaseAsync()
        {
            var database = await _accountManager.GetDatabaseAsync(Account.Id, Id);
            var dialog = _viewModelFactory.CreateDatabaseEditorViewModel(database);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                database = dialog.GetDatabase();
                await _accountManager.UpdateDatabaseAsync(Account.Id, database);
            }
        }
    }
}