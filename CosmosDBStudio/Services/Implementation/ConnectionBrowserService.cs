using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CosmosDBStudio.Services.Implementation
{
    public class ConnectionBrowserService : IConnectionBrowserService
    {
        private readonly IConnectionDirectory _connectionDirectory;

        public ConnectionBrowserService(IConnectionDirectory connectionDirectory)
        {
            _connectionDirectory = connectionDirectory;
        }

        public async Task<string[]> GetDatabasesAsync(string connectionId)
        {
            var connection = _connectionDirectory.GetById(connectionId);
            if (connection == null)
                throw new InvalidOperationException("Connection not found");

            using (var client = CreateDocumentClient(connection))
            {
                var query = client.CreateDatabaseQuery().AsDocumentQuery();
                var databases = new List<string>();
                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<Database>();
                    databases.AddRange(response.Select(d => d.Id));
                }

                return databases.ToArray();
            }
        }

        public async Task<string[]> GetCollectionsAsync(string connectionId, string databaseId)
        {
            var connection = _connectionDirectory.GetById(connectionId);
            if (connection == null)
                throw new InvalidOperationException("Connection not found");

            using (var client = CreateDocumentClient(connection))
            {
                var dbUri = UriFactory.CreateDatabaseUri(databaseId);
                var query = client.CreateDocumentCollectionQuery(dbUri).AsDocumentQuery();
                var collections = new List<string>();
                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<DocumentCollection>();
                    collections.AddRange(response.Select(d => d.Id));
                }

                return collections.ToArray();
            }
        }

        private DocumentClient CreateDocumentClient(DatabaseConnection connection)
        {
            // TODO: connection options
            return new DocumentClient(
                new Uri(connection.Endpoint),
                connection.Key);
        }
    }
}