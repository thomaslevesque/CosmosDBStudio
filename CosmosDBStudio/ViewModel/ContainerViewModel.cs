using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerViewModel : TreeNodeViewModel
    {
        private readonly IMessenger _messenger;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IDialogService _dialogService;

        public DatabaseViewModel Database { get; }
        public string Id { get; }

        public ContainerViewModel(
            DatabaseViewModel database,
            string id,
            IMessenger messenger,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory,
            IDialogService dialogService)
        {
            _messenger = messenger;
            _accountManager = accountManager;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            Database = database;
            Id = id;
            MenuCommands = new[]
            {
                new MenuCommandViewModel(
                    "New query sheet",
                    new DelegateCommand(CreateNewQuerySheet)),
                new MenuCommandViewModel(
                    "Edit container",
                    new AsyncDelegateCommand(EditAsync))
            };
        }

        public override string Text => Id;

        public override IEnumerable<MenuCommandViewModel> MenuCommands { get; }

        public override NonLeafTreeNodeViewModel? Parent => Database;

        private void CreateNewQuerySheet()
        {
            _messenger.Publish(new NewQuerySheetMessage(
                Database.Account.Id,
                Database.Id,
                Id));
        }

        private async Task EditAsync()
        {
            var database = await _accountManager.GetDatabaseAsync(Database.Account.Id, Database.Id);
            var container = await _accountManager.GetContainerAsync(Database.Account.Id, Database.Id, Id);
            var dialog = _viewModelFactory.CreateContainerEditorViewModel(database.Throughput.HasValue, container);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                container = dialog.GetContainer();
                await _accountManager.UpdateContainerAsync(Database.Account.Id, Database.Id, container);
            }
        }
    }
}