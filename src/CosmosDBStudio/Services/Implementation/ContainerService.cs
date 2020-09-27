using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class ContainerService : IContainerService
    {
        private readonly Database _database;

        public ContainerService(Database database)
        {
            _database = database;
        }

        public async Task<string[]> GetContainerNamesAsync(CancellationToken cancellationToken)
        {
            var iterator = _database.GetContainerQueryIterator<ContainerProperties>();
            var containers = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                containers.AddRange(response.Select(d => d.Id));
            }

            return containers.ToArray();
        }

        public async Task<CosmosContainer> GetContainerAsync(string containerId, CancellationToken cancellationToken)
        {
            var container = _database.GetContainer(containerId);
            var properties = (await container.ReadContainerAsync(cancellationToken: cancellationToken)).Resource;
            return new CosmosContainer
            {
                Id = containerId,
                PartitionKeyPath = properties.PartitionKeyPath,
                LargePartitionKey = properties.PartitionKeyDefinitionVersion > PartitionKeyDefinitionVersion.V1,
                DefaultTTL = properties.DefaultTimeToLive,
                ETag = properties.ETag
            };
        }

        public async Task<OperationResult> CreateContainerAsync(CosmosContainer container, int? throughput, CancellationToken cancellationToken)
        {
            var properties = new ContainerProperties(container.Id, container.PartitionKeyPath)
            {
                DefaultTimeToLive = container.DefaultTTL,
                PartitionKeyDefinitionVersion = container.LargePartitionKey
                    ? PartitionKeyDefinitionVersion.V2
                    : PartitionKeyDefinitionVersion.V1
            };

            try
            {
                var response = await _database.CreateContainerAsync(properties, throughput, cancellationToken: cancellationToken);
                container.ETag = response.Resource.ETag;
                return OperationResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return OperationResult.AlreadyExists;
            }
        }

        public async Task<OperationResult> UpdateContainerAsync(CosmosContainer container, CancellationToken cancellationToken)
        {
            var c = _database.GetContainer(container.Id);
            var properties = (await c.ReadContainerAsync(cancellationToken: cancellationToken)).Resource;

            if (container.LargePartitionKey != (properties.PartitionKeyDefinitionVersion > PartitionKeyDefinitionVersion.V1))
                throw new InvalidOperationException("Cannot change partition key definition version for an existing container.");

            if (container.PartitionKeyPath != properties.PartitionKeyPath)
                throw new InvalidOperationException("Cannot change partition key path for an existing container.");

            bool somethingChanged = container.DefaultTTL != properties.DefaultTimeToLive;

            if (somethingChanged)
            {
                properties.DefaultTimeToLive = container.DefaultTTL;
                try
                {
                    var response = await c.ReplaceContainerAsync(properties, new ContainerRequestOptions { IfMatchEtag = container.ETag }, cancellationToken);
                    container.ETag = response.Resource.ETag;
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    return OperationResult.EditConflict;
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return OperationResult.NotFound;
                }
            }

            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteContainerAsync(CosmosContainer container, CancellationToken cancellationToken)
        {
            try
            {
                var c = _database.GetContainer(container.Id);
                await c.DeleteContainerAsync(new ContainerRequestOptions { IfMatchEtag = container.ETag }, cancellationToken);
                return OperationResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return OperationResult.EditConflict;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return OperationResult.NotFound;
            }
        }
    }
}
