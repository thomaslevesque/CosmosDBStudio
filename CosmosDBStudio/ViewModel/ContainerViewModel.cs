using CosmosDBStudio.Commands;
using System.Collections.Generic;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerViewModel : TreeNodeViewModel
    {
        public DatabaseViewModel Database { get; }
        public string Id { get; }

        public ContainerViewModel(
            DatabaseViewModel database,
            string id,
            ContainerCommands containerCommands)
        {
            Database = database;
            Id = id;
            Commands = new[]
            {
                new CommandViewModel("New query sheet", containerCommands.NewQuerySheetCommand, this),
                new CommandViewModel("Create container", containerCommands.CreateCommand, Database),
                new CommandViewModel("Edit container", containerCommands.EditCommand, this),
                new CommandViewModel("Delete container", containerCommands.DeleteCommand, this)
            };
        }

        public override string Text => Id;

        public override IEnumerable<CommandViewModel> Commands { get; }

        public override NonLeafTreeNodeViewModel? Parent => Database;
    }
}