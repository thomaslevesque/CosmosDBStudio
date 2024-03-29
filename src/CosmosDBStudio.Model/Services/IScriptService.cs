﻿using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Model.Services
{
    public interface IScriptService
    {
        Task<CosmosStoredProcedure[]> GetStoredProceduresAsync(CancellationToken cancellationToken);
        Task<CosmosUserDefinedFunction[]> GetUserDefinedFunctionsAsync(CancellationToken cancellationToken);
        Task<CosmosTrigger[]> GetTriggersAsync(CancellationToken cancellationToken);

        Task<OperationResult> CreateStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken);
        Task<OperationResult> CreateUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken);
        Task<OperationResult> CreateTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken);

        Task<OperationResult> ReplaceStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken);
        Task<OperationResult> ReplaceUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken);
        Task<OperationResult> ReplaceTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken);

        Task<OperationResult> DeleteStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken);
        Task<OperationResult> DeleteUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken);
        Task<OperationResult> DeleteTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken);

        Task<StoredProcedureResult> ExecuteStoredProcedureAsync(CosmosStoredProcedure storedProcedure, object? partitionKey, object?[] parameters, CancellationToken cancellationToken);
    }
}
