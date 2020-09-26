using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
{
    public class ContainerDeletedMessage
    {
        public ContainerDeletedMessage(IDatabaseContext context, CosmosContainer container)
        {
            Context = context;
            Container = container;
        }

        public IDatabaseContext Context { get; }
        public CosmosContainer Container { get; }
    }
}
