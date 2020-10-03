using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Threading.Tasks;

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

        public Task<CosmosContainer> GetContainerAsync(CancellationToken cancellationToken) =>
            DatabaseContext.Containers.GetContainerAsync(ContainerId, cancellationToken);

        public Task<int?> GetThroughputAsync(CancellationToken cancellationToken)
        {
            if (DatabaseContext.AccountContext.IsServerless)
                return Task.FromResult(default(int?));
            return _container.ReadThroughputAsync(cancellationToken);
        }

        public async Task<OperationResult> SetThroughputAsync(int? throughput, CancellationToken cancellationToken)
        {
            int? currentThroughput = await _container.ReadThroughputAsync(cancellationToken);
            if (throughput.HasValue != currentThroughput.HasValue)
                return OperationResult.Forbidden;

            if (throughput.HasValue && throughput != currentThroughput)
                await _container.ReplaceThroughputAsync(throughput.Value, cancellationToken: cancellationToken);

            return OperationResult.Success;
        }
    }
}
