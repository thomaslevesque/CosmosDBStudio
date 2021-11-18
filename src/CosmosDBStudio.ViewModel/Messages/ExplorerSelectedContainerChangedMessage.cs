using CosmosDBStudio.ViewModel.TreeNodes;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class ExplorerSelectedContainerChangedMessage
    {
        public ExplorerSelectedContainerChangedMessage(ContainerNodeViewModel? container)
        {
            Container = container;
        }

        public ContainerNodeViewModel? Container { get; }
    }
}
