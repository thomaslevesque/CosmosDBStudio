using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class ContainerContext : IContainerContext
    {
        private readonly Container _container;
        private readonly CosmosAccount _account;
        private readonly Database _database;

        public ContainerContext(
            CosmosAccount account,
            Database database,
            Container container,
            string partitionKeyPath)
        {
            _account = account;
            _database = database;
            _container = container;
            PartitionKeyPath = partitionKeyPath;
            Documents = new DocumentService(container);
            Query = new QueryService(container);
        }

        public string AccountId => _account.Id;

        public string AccountName => _account.Name;

        public string DatabaseId => _database.Id;

        public string ContainerId => _container.Id;

        public string PartitionKeyPath { get; }

        public IDocumentService Documents { get; }

        public IQueryService Query { get; }
    }
}
