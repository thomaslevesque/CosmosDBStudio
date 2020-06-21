using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
{
    public class ContainerDeletedMessage
    {
        public ContainerDeletedMessage(string accountId, string databaseId, CosmosContainer container)
        {
            AccountId = accountId;
            DatabaseId = databaseId;
            Container = container;
        }

        public string AccountId { get; }
        public string DatabaseId { get; }
        public CosmosContainer Container { get; }
    }
}
