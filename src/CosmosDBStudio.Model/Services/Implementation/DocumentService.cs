using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hamlet;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly Func<Container> _containerGetter;

        public DocumentService(Func<Container> containerGetter)
        {
            _containerGetter = containerGetter;
        }

        public async Task<JObject?> GetAsync(string id, Option<object?> partitionKey, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _containerGetter().ReadItemAsync<JObject>(
                    id,
                    PartitionKeyHelper.Create(partitionKey) ?? PartitionKey.None,
                    null,
                    cancellationToken);

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<JObject> CreateAsync(JObject document, Option<object?> partitionKey, CancellationToken cancellationToken)
        {
            var response = await _containerGetter().CreateItemAsync(
                document,
                PartitionKeyHelper.Create(partitionKey),
                null,
                cancellationToken);

            return response.Resource;
        }

        public async Task<JObject> ReplaceAsync(string id, JObject document, Option<object?> partitionKey, string? eTag, CancellationToken cancellationToken)
        {
            var options = new ItemRequestOptionsBuilder()
                .IfMatch(eTag)
                .Build();

            var response = await _containerGetter().ReplaceItemAsync(
                document,
                id,
                PartitionKeyHelper.Create(partitionKey),
                options,
                cancellationToken);

            return response.Resource;
        }

        public async Task DeleteAsync(string id, Option<object?> partitionKey, string? eTag, CancellationToken cancellationToken)
        {
            var options = new ItemRequestOptionsBuilder()
                .IfMatch(eTag)
                .Build();

            await _containerGetter().DeleteItemAsync<JObject>(
                id,
                PartitionKeyHelper.Create(partitionKey) ?? PartitionKey.None,
                options,
                cancellationToken);
        }
    }
}
