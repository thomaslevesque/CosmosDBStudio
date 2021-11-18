using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public class TriggerNodeViewModel : ScriptNodeViewModel<CosmosTrigger>
    {
        public TriggerNodeViewModel(
            CosmosTrigger trigger,
            IContainerContext context,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosTrigger> commands,
            IMessenger messenger)
            : base(trigger, context, parent, commands, messenger)
        {
        }

        public override string Description => "trigger";

        public override Task DeleteAsync() => Context.Scripts.DeleteTriggerAsync(Script, default);
    }
}
