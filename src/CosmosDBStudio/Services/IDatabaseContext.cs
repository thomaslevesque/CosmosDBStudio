using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IDatabaseContext
    {
        IAccountContext AccountContext { get; }
        string AccountId { get; }
        string AccountName { get; }
        string DatabaseId { get; }
        IContainerService Containers { get; }
        Task<IContainerContext> GetContainerContextAsync(string containerId, CancellationToken cancellationToken);
    }
}
