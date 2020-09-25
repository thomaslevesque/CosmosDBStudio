using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class TriggerViewModel : ContainerScriptViewModel<CosmosTrigger>
    {
        public TriggerViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosTrigger trigger,
            IMessenger messenger)
            : base(container, parent, trigger, messenger)
        {
        }
    }
}
