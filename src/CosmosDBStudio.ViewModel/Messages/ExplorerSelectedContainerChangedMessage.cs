using CosmosDBStudio.ViewModel;

namespace CosmosDBStudio.Messages
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
