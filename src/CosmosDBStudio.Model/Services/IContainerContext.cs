using CosmosDBStudio.Model;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IContainerContext
    {
        IDatabaseContext DatabaseContext { get; }
        string AccountId { get; }
        string AccountName { get; }
        string DatabaseId { get; }
        string ContainerId { get; }
        string Path => $"{AccountName}/{DatabaseId}/{ContainerId}";
        string? PartitionKeyPath { get; }
        string? PartitionKeyJsonPath { get; }
        IDocumentService Documents { get; }
        IQueryService Query { get; }
        IScriptService Scripts { get; }

        Task<CosmosContainer> GetContainerAsync(CancellationToken cancellationToken);

        Task<int?> GetThroughputAsync(CancellationToken cancellationToken);
        Task<OperationResult> SetThroughputAsync(int? throughput, CancellationToken cancellationToken);
    }
}
