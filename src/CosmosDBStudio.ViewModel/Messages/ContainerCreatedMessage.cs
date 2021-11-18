using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
{
    public class ContainerCreatedMessage
    {
        public ContainerCreatedMessage(IDatabaseContext context, CosmosContainer container)
        {
            Context = context;
            Container = container;
        }

        public IDatabaseContext Context { get; }
        public CosmosContainer Container { get; }
    }
}
