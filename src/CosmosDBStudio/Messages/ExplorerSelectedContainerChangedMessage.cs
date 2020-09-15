using CosmosDBStudio.ViewModel;

namespace CosmosDBStudio.Messages
{
    public class ExplorerSelectedContainerChangedMessage
    {
        public ExplorerSelectedContainerChangedMessage(ContainerViewModel? container)
        {
            Container = container;
        }

        public ContainerViewModel? Container { get; }
    }
}
