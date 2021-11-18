using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Model.Services
{
    public interface IDatabaseService
    {
        Task<CosmosDatabase[]> GetDatabasesAsync(CancellationToken cancellationToken);
        Task<CosmosDatabase> GetDatabaseAsync(string databaseId, CancellationToken cancellationToken);
        Task<OperationResult> CreateDatabaseAsync(CosmosDatabase database, int? throughput, CancellationToken cancellationToken);
        Task<OperationResult> DeleteDatabaseAsync(CosmosDatabase database, CancellationToken cancellationToken);
    }
}
