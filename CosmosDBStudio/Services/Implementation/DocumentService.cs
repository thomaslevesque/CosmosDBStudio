using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly Container _container;

        public DocumentService(Container container)
        {
            _container = container;
        }

        public async Task<JObject?> GetAsync(string id, object? partitionKey, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _container.ReadItemAsync<JObject>(
                    id,
                    PartitionKeyHelper.Create(partitionKey) ?? PartitionKey.None,
                    null,
                    cancellationToken);

                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<JObject> CreateAsync(JObject document, object? partitionKey, CancellationToken cancellationToken)
        {
            var response = await _container.CreateItemAsync(
                document,
                PartitionKeyHelper.Create(partitionKey),
                null,
                cancellationToken);

            return response.Resource;
        }

        public async Task<JObject> ReplaceAsync(string id, JObject document, object? partitionKey, string? eTag, CancellationToken cancellationToken)
        {
            var options = new ItemRequestOptionsBuilder()
                .IfMatch(eTag)
                .Build();

            var response = await _container.ReplaceItemAsync(
                document,
                id,
                PartitionKeyHelper.Create(partitionKey),
                options,
                cancellationToken);

            return response.Resource;
        }

        public async Task DeleteAsync(string id, object? partitionKey, string? eTag, CancellationToken cancellationToken)
        {
            var options = new ItemRequestOptionsBuilder()
                .IfMatch(eTag)
                .Build();

            await _container.DeleteItemAsync<JObject>(
                id,
                PartitionKeyHelper.Create(partitionKey) ?? PartitionKey.None,
                options,
                cancellationToken);
        }
    }
}
