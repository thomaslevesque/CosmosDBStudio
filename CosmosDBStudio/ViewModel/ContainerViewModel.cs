using System.Collections.Generic;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerViewModel : TreeNodeViewModel
    {
        private readonly IMessenger _messenger;

        public DatabaseViewModel Database { get; }
        public string Id { get; }

        public ContainerViewModel(DatabaseViewModel database, string id, IMessenger messenger)
        {
            _messenger = messenger;
            Database = database;
            Id = id;
            MenuCommands = new[]
            {
                new MenuCommandViewModel(
                    "New query sheet",
                    new DelegateCommand(CreateNewQuerySheet)) 
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
    }
}