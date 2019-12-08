using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IDocumentService
    {
        Task<JObject> CreateAsync(JObject document, object? partitionKey, CancellationToken cancellationToken);
        Task<JObject?> GetAsync(string id, object? partitionKey, CancellationToken cancellationToken);
        Task<JObject> ReplaceAsync(string id, JObject document, object? partitionKey, string? eTag, CancellationToken cancellationToken);
        Task DeleteAsync(string id, object? partitionKey, string? eTag, CancellationToken cancellationToken);
    }
}
