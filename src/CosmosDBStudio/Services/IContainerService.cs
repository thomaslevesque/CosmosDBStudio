using CosmosDBStudio.Model;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IContainerService
    {
        Task<string[]> GetContainerNamesAsync(CancellationToken cancellationToken);
        Task<CosmosContainer> GetContainerAsync(string containerId, CancellationToken cancellationToken);
        Task<OperationResult> CreateContainerAsync(CosmosContainer container, int? throughput, CancellationToken cancellationToken);
        Task<OperationResult> UpdateContainerAsync(CosmosContainer container, CancellationToken cancellationToken);
        Task<OperationResult> DeleteContainerAsync(CosmosContainer container, CancellationToken cancellationToken);
    }
}
