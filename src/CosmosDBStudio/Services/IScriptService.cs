using CosmosDBStudio.Model;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IScriptService
    {
        Task<ScriptSaveResult> CreateStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken);
        Task<ScriptSaveResult> CreateUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken);
        Task<ScriptSaveResult> CreateTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken);

        Task<ScriptSaveResult> ReplaceStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken);
        Task<ScriptSaveResult> ReplaceUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken);
        Task<ScriptSaveResult> ReplaceTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken);
    }
}
