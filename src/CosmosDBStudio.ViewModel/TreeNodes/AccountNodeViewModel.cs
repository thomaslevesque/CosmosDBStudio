using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;
using CosmosDBStudio.ViewModel.Messages;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public class AccountNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IClientPool _clientPool;

        public AccountNodeViewModel(
            CosmosAccount account,
            IAccountContext context,
            AccountFolderNodeViewModel? parent,
            AccountCommands accountCommands,
            DatabaseCommands databaseCommands,
            IViewModelFactory viewModelFactory,
            IMessenger messenger,
            IClientPool clientPool)
        {
            Id = account.Id;
            Context = context;
            Parent = parent;
            _viewModelFactory = viewModelFactory;
            _clientPool = clientPool;
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

            messenger.Subscribe(this).To<AccountEditedMessage>((vm, message) => vm.OnAccountEdited(message));
            messenger.Subscribe(this).To<DatabaseCreatedMessage>((vm, message) => vm.OnDatabaseCreated(message));
            messenger.Subscribe(this).To<DatabaseDeletedMessage>((vm, message) => vm.OnDatabaseDeleted(message));
        }

        public string Id { get; }

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
            var databases = await Context.Databases.GetDatabasesAsync(default);
            return databases
                .Select(database => _viewModelFactory.CreateDatabaseNode(this, database, Context.GetDatabaseContext(database)))
                .ToList();
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        public IAccountContext Context { get; }

        private void OnAccountEdited(AccountEditedMessage message)
        {
            if (message.Account.Id != Id)
                return;

            Name = message.Account.Name;
            if (message.CredentialsChanged)
            {
                _clientPool.RemoveClientForAccount(message.Account);
                ReloadChildren();
            }
        }

        private void OnDatabaseCreated(DatabaseCreatedMessage message)
        {
            if (message.Context.AccountId != Id)
                return;

            ReloadChildren(); // TODO: improve this
        }

        private void OnDatabaseDeleted(DatabaseDeletedMessage message)
        {
            if (message.Context.AccountId != Id)
                return;

            ReloadChildren(); // TODO: improve this
        }
    }
}