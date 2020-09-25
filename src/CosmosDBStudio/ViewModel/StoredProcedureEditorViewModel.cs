using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureEditorViewModel : ScriptEditorViewModelBase<CosmosStoredProcedure>
    {
        public StoredProcedureEditorViewModel(IContainerContext containerContext, CosmosStoredProcedure script)
            : base(containerContext, script)
        {
        }

        public override string Description => "stored procedure";

        protected override Task CreateScriptAsync(CosmosStoredProcedure script) =>
            ContainerContext.Scripts.CreateStoredProcedureAsync(script, default);

        protected override Task ReplaceScriptAsync(CosmosStoredProcedure script) =>
            ContainerContext.Scripts.ReplaceStoredProcedureAsync(script, default);
    }
}
