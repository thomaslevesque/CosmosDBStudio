using CosmosDBStudio.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerViewModel : NonLeafTreeNodeViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseViewModel Database { get; }
        public string Id { get; }

        public ContainerViewModel(
            DatabaseViewModel database,
            string id,
            ContainerCommands containerCommands,
            IViewModelFactory viewModelFactory)
        {
            Database = database;
            Id = id;
            _viewModelFactory = viewModelFactory;
            Commands = new[]
            {
                new CommandViewModel("New query sheet", containerCommands.NewQuerySheetCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Create container", containerCommands.CreateCommand, Database),
                new CommandViewModel("Edit container", containerCommands.EditCommand, this),
                new CommandViewModel("Delete container", containerCommands.DeleteCommand, this),
            };
        }

        public override string Text => Id;

        public override IEnumerable<CommandViewModel> Commands { get; }

        public override NonLeafTreeNodeViewModel? Parent => Database;

        public string Path => $"{Database.Account.Name}/{Database.Id}/{Id}";

        protected override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            return Task.FromResult(GetChildren());

            IEnumerable<TreeNodeViewModel> GetChildren()
            {
                yield return _viewModelFactory.CreateStoredProceduresFolder(this);
                yield return _viewModelFactory.CreateUserDefinedFunctionsFolder(this);
                yield return _viewModelFactory.CreateTriggersFolder(this);
            }
        }
    }
}