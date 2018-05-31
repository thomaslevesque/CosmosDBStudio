using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Services.Implementation
{
    public class QueryExecutionService : IQueryExecutionService
    {
        private readonly IConnectionDirectory _connectionDirectory;

        public QueryExecutionService(IConnectionDirectory connectionDirectory)
        {
            _connectionDirectory = connectionDirectory;
        }

        public async Task<QueryResult> ExecuteAsync(Query query)
        {
            if (string.IsNullOrEmpty(query.ConnectionId))
            {
                throw new ArgumentException("No connection specified");
            }

            if (string.IsNullOrEmpty(query.DatabaseId))
            {
                throw new ArgumentException("No database specified");
            }

            if (string.IsNullOrEmpty(query.CollectionId))
            {
                throw new ArgumentException("No collection specified");
            }

            var connection = _connectionDirectory.GetById(query.ConnectionId);
            if (connection == null)
            {
                throw new InvalidOperationException("Connection not found");
            }

            using (var client = CreateDocumentClient(connection))
            {
                var collectionUri = UriFactory.CreateDocumentCollectionUri(query.DatabaseId, query.CollectionId);
                var feedOptions = CreateFeedOptions(query.Options);
                var querySpec = CreateQuerySpec(query);
                var documentQuery = client.CreateDocumentQuery<Document>(collectionUri, querySpec, feedOptions)
                    .AsDocumentQuery();

                var result = new QueryResult();
                var stopwatch = new Stopwatch();
                try
                {
                    var documents = new List<Document>();
                    while (documentQuery.HasMoreResults)
                    {
                        stopwatch.Start();
                        var response = await documentQuery.ExecuteNextAsync<Document>();
                        stopwatch.Stop();
                        result.RequestCharge += response.RequestCharge;
                        documents.AddRange(response);
                    }
                    result.Documents = documents.Select(DocumentToJObject).ToList();
                }
                catch (DocumentClientException ex)
                {
                    result.Error = ex;
                }
                finally
                {
                    stopwatch.Stop();
                }

                result.TimeElapsed = stopwatch.Elapsed;
                return result;
            }
        }

        private JObject DocumentToJObject(Document doc)
        {
            var bytes = doc.ToByteArray();
            using (var ms = new MemoryStream(bytes))
            using (var streamReader = new StreamReader(ms))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return JObject.Load(jsonReader);
            }
        }

        private DocumentClient CreateDocumentClient(DatabaseConnection connection)
        {
            // TODO: connection options
            return new DocumentClient(
                new Uri(connection.Endpoint),
                connection.Key);
        }

        private FeedOptions CreateFeedOptions(QueryOptions options)
        {
            var feedOptions = new FeedOptions();
            if (options?.PartitionKey == null)
            {
                feedOptions.EnableCrossPartitionQuery = true;
            }
            else
            {
                feedOptions.PartitionKey = new PartitionKey(options.PartitionKey);
            }

            return feedOptions;
        }

        private SqlQuerySpec CreateQuerySpec(Query query)
        {
            var querySpec = new SqlQuerySpec(query.Sql);
            if (query.Parameters != null && query.Parameters.Count > 0)
                querySpec.Parameters = new SqlParameterCollection(
                    query.Parameters.Select(kvp => new SqlParameter(kvp.Key, kvp.Value)));
            return querySpec;
        }
    }
}