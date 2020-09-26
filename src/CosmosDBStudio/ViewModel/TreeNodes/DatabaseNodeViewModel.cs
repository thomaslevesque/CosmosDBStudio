using CosmosDBStudio.Commands;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseNodeViewModel(
            AccountNodeViewModel account,
            string id,
            IDatabaseContext context,
            DatabaseCommands databaseCommands,
            ContainerCommands containerCommands,
            IViewModelFactory viewModelFactory,
            IMessenger messenger)
        {
            Account = account;
            Id = id;
            Context = context;
            _viewModelFactory = viewModelFactory;

            Commands = new[]
            {
                new CommandViewModel("Create container", containerCommands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand),
                CommandViewModel.Separator(),
                new CommandViewModel("Create database", databaseCommands.CreateCommand, Account),
                new CommandViewModel("Edit database", databaseCommands.EditCommand, this),
                new CommandViewModel("Delete database", databaseCommands.DeleteCommand, this),
            };

            messenger.Subscribe(this).To<ContainerCreatedMessage>((vm, message) => vm.OnContainerCreated(message));
            messenger.Subscribe(this).To<ContainerDeletedMessage>((vm, message) => vm.OnContainerDeleted(message));
        }

        public AccountNodeViewModel Account { get; }

        public IDatabaseContext Context { get; }

        public string Id { get; }

        public override string Text => Id;

        public override NonLeafTreeNodeViewModel? Parent => Account;

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var containers = await Context.Containers.GetContainerNamesAsync(default);
            var vms = new List<ContainerNodeViewModel>();
            foreach (var id in containers)
            {
                var context = await Context.GetContainerContextAsync(id, default);
                vms.Add(_viewModelFactory.CreateContainerNode(this, id, context));
            }
            return vms;
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        private void OnContainerCreated(ContainerCreatedMessage message)
        {
            if ((message.Context.AccountId, message.Context.DatabaseId) != (Account.Id, Id))
                return;

            ReloadChildren(); // TODO: improve this
        }

        private void OnContainerDeleted(ContainerDeletedMessage message)
        {
            if ((message.Context.AccountId, message.Context.DatabaseId) != (Account.Id, Id))
                return;

            ReloadChildren(); // TODO: improve this
        }
    }
}