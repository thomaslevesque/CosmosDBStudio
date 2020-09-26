using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProceduresFolderNodeViewModel : ScriptFolderNodeViewModel
    {
        public StoredProceduresFolderNodeViewModel(
            IContainerContext context,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosStoredProcedure> commands,
            IViewModelFactory viewModelFactory)
            : base("Stored procedures", context, parent, viewModelFactory)
        {
            Commands = new[]
            {
                new CommandViewModel($"New stored procedure", commands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand)
            };
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var storedProcedures = await Context.Scripts.GetStoredProceduresAsync(default);
            return storedProcedures.Select(sp => ViewModelFactory.CreateStoredProcedureNode(sp, Context, this));
        }
    }
}
