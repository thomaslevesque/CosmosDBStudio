using CosmosDBStudio.Commands;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class AccountViewModel : NonLeafTreeNodeViewModel
    {
        private readonly CosmosAccount _account;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IViewModelFactory _viewModelFactory;

        public AccountViewModel(
            CosmosAccount account,
            AccountFolderViewModel? parent,
            AccountCommands accountCommands,
            DatabaseCommands databaseCommands,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory,
            IMessenger messenger)
        {
            _account = account;
            Parent = parent;
            _accountManager = accountManager;
            _viewModelFactory = viewModelFactory;
            _name = account.Name;

            Commands = new[]
            {
                new CommandViewModel("Create database", databaseCommands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand),
                CommandViewModel.Separator(),
                new CommandViewModel("Add account", accountCommands.AddCommand, Parent),
                new CommandViewModel("Edit account", accountCommands.EditCommand, this),
                new CommandViewModel("Remove account", accountCommands.RemoveCommand, this),
            };

            messenger.Subscribe(this).To<DatabaseCreatedMessage>((vm, message) => vm.OnDatabaseCreated(message));
            messenger.Subscribe(this).To<DatabaseDeletedMessage>((vm, message) => vm.OnDatabaseDeleted(message));
        }

        public string Id => _account.Id;

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value).AndNotifyPropertyChanged(nameof(Text));
        }

        public override string Text => _name;

        public override NonLeafTreeNodeViewModel? Parent { get; }

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var databases = await _accountManager.GetDatabasesAsync(_account.Id);
            return databases.Select(id => _viewModelFactory.CreateDatabaseViewModel(this, id)).ToList();
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        private void OnDatabaseCreated(DatabaseCreatedMessage message)
        {
            if (message.AccountId != Id)
                return;

            ReloadChildren(); // TODO: improve this
        }

        private void OnDatabaseDeleted(DatabaseDeletedMessage message)
        {
            if (message.AccountId != Id)
                return;

            ReloadChildren(); // TODO: improve this
        }
    }
}