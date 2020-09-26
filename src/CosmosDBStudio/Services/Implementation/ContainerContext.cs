using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class ContainerContext : IContainerContext
    {
        private readonly Container _container;
        private readonly Database _database;

        public ContainerContext(
            IDatabaseContext databaseContext,
            Database database,
            Container container,
            string partitionKeyPath)
        {
            DatabaseContext = databaseContext;
            _database = database;
            _container = container;
            PartitionKeyPath = partitionKeyPath;
            PartitionKeyJsonPath = string.IsNullOrEmpty(partitionKeyPath)
                ? null
                : "$" + partitionKeyPath.Replace('/', '.');
            Documents = new DocumentService(container);
            Query = new QueryService(container);
            Scripts = new ScriptService(container);
        }

        public IDatabaseContext DatabaseContext { get; }

        public string AccountId => DatabaseContext.AccountId;

        public string AccountName => DatabaseContext.AccountName;

        public string DatabaseId => _database.Id;

        public string ContainerId => _container.Id;

        public string? PartitionKeyPath { get; }
        public string? PartitionKeyJsonPath { get; }

        public IDocumentService Documents { get; }

        public IQueryService Query { get; }

        public IScriptService Scripts { get; }
    }
}
