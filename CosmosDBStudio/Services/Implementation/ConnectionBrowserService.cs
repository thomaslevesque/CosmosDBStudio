using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

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

            using var client = CreateCosmosClient(connection);
            var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
            var databases = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                databases.AddRange(response.Select(d => d.Id));
            }

            return databases.ToArray();
        }

        public async Task<string[]> GetContainersAsync(string connectionId, string databaseId)
        {
            var connection = _connectionDirectory.GetById(connectionId);
            if (connection == null)
                throw new InvalidOperationException("Connection not found");

            using var client = CreateCosmosClient(connection);
            var database = client.GetDatabase(databaseId);
            var iterator = database.GetContainerQueryIterator<ContainerProperties>();
            var containers = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                containers.AddRange(response.Select(d => d.Id));
            }

            return containers.ToArray();
        }

        private CosmosClient CreateCosmosClient(DatabaseConnection connection)
        {
            // TODO: connection options
            return new CosmosClient(
                connection.Endpoint,
                connection.Key);
        }
    }
}