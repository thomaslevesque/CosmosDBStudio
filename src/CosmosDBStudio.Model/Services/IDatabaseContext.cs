using CosmosDBStudio.Model;
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

        IContainerContext GetContainerContext(CosmosContainer container, CancellationToken cancellationToken);

        Task<CosmosDatabase> GetDatabaseAsync(CancellationToken cancellationToken);

        Task<int?> GetThroughputAsync(CancellationToken cancellationToken);
        Task<OperationResult> SetThroughputAsync(int? throughput, CancellationToken cancellationToken);
    }
}
