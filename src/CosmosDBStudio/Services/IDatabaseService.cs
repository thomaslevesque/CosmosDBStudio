﻿using CosmosDBStudio.Model;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IDatabaseService
    {
        Task<string[]> GetDatabaseNamesAsync(CancellationToken cancellationToken);
        Task<CosmosDatabase> GetDatabaseAsync(string databaseId, CancellationToken cancellationToken);
        Task<OperationResult> CreateDatabaseAsync(CosmosDatabase database, CancellationToken cancellationToken);
        Task<OperationResult> UpdateDatabaseAsync(CosmosDatabase database, CancellationToken cancellationToken);
        Task<OperationResult> DeleteDatabaseAsync(CosmosDatabase database, CancellationToken cancellationToken);
    }
}
