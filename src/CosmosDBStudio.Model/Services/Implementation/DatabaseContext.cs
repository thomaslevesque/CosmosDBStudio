using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly Func<Database> _databaseGetter;

        public DatabaseContext(IAccountContext accountContext, string databaseId, Func<Database> databaseGetter)
        {
            AccountContext = accountContext;
            DatabaseId = databaseId;
            _databaseGetter = databaseGetter;
            Containers = new ContainerService(databaseGetter);
        }

        public IAccountContext AccountContext { get; }

        public string AccountId => AccountContext.AccountId;

        public string AccountName => AccountContext.AccountName;

        public string DatabaseId { get; }

        public IContainerService Containers { get; }

        public IContainerContext GetContainerContext(CosmosContainer container, CancellationToken cancellationToken)
        {
            return new ContainerContext(this, container.Id, () => _databaseGetter().GetContainer(container.Id), container.PartitionKeyPath);
        }

        public Task<CosmosDatabase> GetDatabaseAsync(CancellationToken cancellationToken) =>
            AccountContext.Databases.GetDatabaseAsync(DatabaseId, cancellationToken);

        public Task<int?> GetThroughputAsync(CancellationToken cancellationToken)
        {
            var database = _databaseGetter();
            if (AccountContext.IsServerless)
                return Task.FromResult(default(int?));
            return database.ReadThroughputAsync(cancellationToken);
        }

        public async Task<OperationResult> SetThroughputAsync(int? throughput, CancellationToken cancellationToken)
        {
            var database = _databaseGetter();
            int? currentThroughput = await database.ReadThroughputAsync(cancellationToken);
            if (throughput.HasValue != currentThroughput.HasValue)
                return OperationResult.Forbidden;

            if (throughput.HasValue && throughput != currentThroughput)
                await database.ReplaceThroughputAsync(throughput.Value, cancellationToken: cancellationToken);

            return OperationResult.Success;
        }
    }
}
