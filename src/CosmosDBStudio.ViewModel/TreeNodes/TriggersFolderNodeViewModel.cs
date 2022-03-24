using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;

namespace CosmosDBStudio.ViewModel.TreeNodes
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

        protected override async Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var triggers = await Context.Scripts.GetTriggersAsync(default);
            return triggers.OrderBy(t => ((ITreeNode)t).DisplayName).Select(t => ViewModelFactory.CreateTriggerNode(t, Context, this));
        }
    }
}
