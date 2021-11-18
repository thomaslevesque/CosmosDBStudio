using System.Threading;
using System.Threading.Tasks;
using Hamlet;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Model.Services
{
    public interface IDocumentService
    {
        Task<JObject> CreateAsync(JObject document, Option<object?> partitionKey, CancellationToken cancellationToken);
        Task<JObject?> GetAsync(string id, Option<object?> partitionKey, CancellationToken cancellationToken);
        Task<JObject> ReplaceAsync(string id, JObject document, Option<object?> partitionKey, string? eTag, CancellationToken cancellationToken);
        Task DeleteAsync(string id, Option<object?> partitionKey, string? eTag, CancellationToken cancellationToken);
    }
}
