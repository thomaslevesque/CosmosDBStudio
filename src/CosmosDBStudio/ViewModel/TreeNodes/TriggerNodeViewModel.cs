using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class TriggerNodeViewModel : ScriptNodeViewModel<CosmosTrigger>
    {
        public TriggerNodeViewModel(
            ContainerNodeViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosTrigger trigger,
            ScriptCommands<CosmosTrigger> commands,
            IMessenger messenger)
            : base(container, parent, trigger, commands, messenger)
        {
        }

        public override string Description => "trigger";

        public override Task DeleteAsync(ICosmosAccountManager accountManager)
        {
            return accountManager.DeleteTriggerAsync(
                Container.Database.Account.Id,
                Container.Database.Id,
                Container.Id,
                Script);
        }
    }
}
