using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class ContainerContext : IContainerContext
    {
        private readonly Func<Container> _containerGetter;

        public ContainerContext(
            IDatabaseContext databaseContext,
            string containerId,
            Func<Container> containerGetter,
            string partitionKeyPath)
        {
            DatabaseContext = databaseContext;
            ContainerId = containerId;
            _containerGetter = containerGetter;
            PartitionKeyPath = partitionKeyPath;
            PartitionKeyJsonPath = string.IsNullOrEmpty(partitionKeyPath)
                ? null
                : "$" + partitionKeyPath.Replace('/', '.');
            Documents = new DocumentService(containerGetter);
            Query = new QueryService(containerGetter);
            Scripts = new ScriptService(containerGetter);
        }

        public IDatabaseContext DatabaseContext { get; }

        public string AccountId => DatabaseContext.AccountId;

        public string AccountName => DatabaseContext.AccountName;

        public string DatabaseId => DatabaseContext.DatabaseId;

        public string ContainerId { get; }

        public string? PartitionKeyPath { get; }
        public string? PartitionKeyJsonPath { get; }

        public IDocumentService Documents { get; }

        public IQueryService Query { get; }

        public IScriptService Scripts { get; }

        public Task<CosmosContainer> GetContainerAsync(CancellationToken cancellationToken) =>
            DatabaseContext.Containers.GetContainerAsync(ContainerId, cancellationToken);

        public Task<int?> GetThroughputAsync(CancellationToken cancellationToken)
        {
            var container = _containerGetter();
            if (DatabaseContext.AccountContext.IsServerless)
                return Task.FromResult(default(int?));
            return container.ReadThroughputAsync(cancellationToken);
        }

        public async Task<OperationResult> SetThroughputAsync(int? throughput, CancellationToken cancellationToken)
        {
            var container = _containerGetter();
            int? currentThroughput = await container.ReadThroughputAsync(cancellationToken);
            if (throughput.HasValue != currentThroughput.HasValue)
                return OperationResult.Forbidden;

            if (throughput.HasValue && throughput != currentThroughput)
                await container.ReplaceThroughputAsync(throughput.Value, cancellationToken: cancellationToken);

            return OperationResult.Success;
        }
    }
}
