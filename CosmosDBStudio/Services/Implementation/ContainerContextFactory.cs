using System;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class ContainerContextFactory : IContainerContextFactory
    {
        private readonly IAccountDirectory _accountDirectory;
        private readonly IClientPool _clientPool;

        public ContainerContextFactory(IAccountDirectory accountDirectory, IClientPool clientPool)
        {
            _accountDirectory = accountDirectory;
            _clientPool = clientPool;
        }

        public async Task<IContainerContext> CreateAsync(string accountId, string databaseId, string containerId, CancellationToken cancellationToken)
        {
            if (!_accountDirectory.TryGetById(accountId, out var account))
                throw new InvalidOperationException("Account not found");
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var container = database.GetContainer(containerId);
            var containerResponse = await container.ReadContainerAsync(null, cancellationToken);
            var partitionKeyPath = containerResponse.Resource.PartitionKeyPath;
            return new ContainerContext(account, database, container, partitionKeyPath);
        }
    }
}
