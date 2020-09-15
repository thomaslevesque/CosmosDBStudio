using CosmosDBStudio.Commands;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseViewModel : NonLeafTreeNodeViewModel
    {
        private readonly ICosmosAccountManager _accountManager;
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseViewModel(
            AccountViewModel account,
            string id,
            DatabaseCommands databaseCommands,
            ContainerCommands containerCommands,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory,
            IMessenger messenger)
        {
            Account = account;
            Id = id;
            _accountManager = accountManager;
            _viewModelFactory = viewModelFactory;

            Commands = new[]
            {
                new CommandViewModel("Create container", containerCommands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Create database", databaseCommands.CreateCommand, Account),
                new CommandViewModel("Edit database", databaseCommands.EditCommand, this),
                new CommandViewModel("Delete database", databaseCommands.DeleteCommand, this),
            };

            messenger.Subscribe(this).To<ContainerCreatedMessage>((vm, message) => vm.OnContainerCreated(message));
            messenger.Subscribe(this).To<ContainerDeletedMessage>((vm, message) => vm.OnContainerDeleted(message));
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

        public override IEnumerable<CommandViewModel> Commands { get; }

        private void OnContainerCreated(ContainerCreatedMessage message)
        {
            if ((message.AccountId, message.DatabaseId) != (Account.Id, Id))
                return;

            ReloadChildren(); // TODO: improve this
        }

        private void OnContainerDeleted(ContainerDeletedMessage message)
        {
            if ((message.AccountId, message.DatabaseId) != (Account.Id, Id))
                return;

            ReloadChildren(); // TODO: improve this
        }
    }
}