using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public class TriggerViewModel : ContainerScriptViewModel<CosmosTrigger>
    {
        public TriggerViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosTrigger trigger)
            : base(container, parent, trigger)
        {
        }
    }
}
