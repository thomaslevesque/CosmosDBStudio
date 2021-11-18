using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public class ContainerNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly ContainerCommands _containerCommands;
        private readonly IViewModelFactory _viewModelFactory;

        public DatabaseNodeViewModel Database { get; }
        public string Id { get; }

        public ContainerNodeViewModel(
            DatabaseNodeViewModel database,
            CosmosContainer container,
            IContainerContext context,
            ContainerCommands containerCommands,
            IViewModelFactory viewModelFactory)
        {
            Database = database;
            Id = container.Id;
            Context = context;
            _containerCommands = containerCommands;
            _viewModelFactory = viewModelFactory;
            Commands = new[]
            {
                new CommandViewModel("New query sheet", containerCommands.NewQuerySheetCommand, this, isDefault: true),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand),
                CommandViewModel.Separator(),
                new CommandViewModel("Create container", containerCommands.CreateCommand, Database),
                new CommandViewModel("Edit container", containerCommands.EditCommand, this),
                new CommandViewModel("Delete container", containerCommands.DeleteCommand, this),
            };
        }

        private DelegateCommand? _newQuerySheetCommand;
        public ICommand NewQuerySheetCommand => _newQuerySheetCommand ??= _containerCommands.NewQuerySheetCommand.WithParameter(this);

        public override string Text => Id;

        public override IEnumerable<CommandViewModel> Commands { get; }

        public override NonLeafTreeNodeViewModel? Parent => Database;

        public string Path => $"{Database.Account.Name}/{Database.Id}/{Id}";

        public IContainerContext Context { get; }

        protected override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            return Task.FromResult(GetChildren());

            IEnumerable<TreeNodeViewModel> GetChildren()
            {
                yield return _viewModelFactory.CreateStoredProceduresFolderNode(Context, this);
                yield return _viewModelFactory.CreateUserDefinedFunctionsFolderNode(Context, this);
                yield return _viewModelFactory.CreateTriggersFolderNode(Context, this);
            }
        }
    }
}