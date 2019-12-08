using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IContainerContextFactory
    {
        Task<IContainerContext> CreateAsync(string accountId, string databaseId, string containerId, CancellationToken cancellationToken);
    }
}
