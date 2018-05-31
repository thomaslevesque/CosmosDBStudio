using System.Collections.Generic;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class CollectionViewModel : TreeNodeViewModel
    {
        private readonly IMessenger _messenger;

        public DatabaseViewModel Database { get; }
        public string Id { get; }

        public CollectionViewModel(DatabaseViewModel database, string id, IMessenger messenger)
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

        private void CreateNewQuerySheet()
        {
            _messenger.Publish(new NewQuerySheetMessage(
                Database.Connection.Id,
                Database.Id,
                Id));
        }
    }
}