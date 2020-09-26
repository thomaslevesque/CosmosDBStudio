using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly Database _database;

        public DatabaseContext(IAccountContext accountContext, Database database)
        {
            AccountContext = accountContext;
            _database = database;
            Containers = new ContainerService(database);
        }

        public IAccountContext AccountContext { get; }

        public string AccountId => AccountContext.AccountId;

        public string AccountName => AccountContext.AccountName;

        public string DatabaseId => _database.Id;

        public IContainerService Containers { get; }

        public async Task<IContainerContext> GetContainerContextAsync(string containerId, CancellationToken cancellationToken)
        {
            var container = _database.GetContainer(containerId);
            var containerResponse = await container.ReadContainerAsync(null, cancellationToken);
            var partitionKeyPath = containerResponse.Resource.PartitionKeyPath;
            return new ContainerContext(this, _database, container, partitionKeyPath);
        }
    }
}
