using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class TriggersFolderNodeViewModel : ScriptFolderNodeViewModel
    {
        public TriggersFolderNodeViewModel(
            IContainerContext containerContext,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosTrigger> commands,
            IViewModelFactory viewModelFactory)
            : base("Triggers", containerContext, parent, viewModelFactory)
        {
            Commands = new[]
            {
                new CommandViewModel($"New trigger", commands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand)
            };
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var triggers = await Context.Scripts.GetTriggersAsync(default);
            return triggers.Select(t => ViewModelFactory.CreateTriggerNode(t, Context, this));
        }
    }
}
