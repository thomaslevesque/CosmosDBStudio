using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureNodeViewModel : ScriptNodeViewModel<CosmosStoredProcedure>
    {
        public StoredProcedureNodeViewModel(
            IContainerContext context,
            CosmosStoredProcedure storedProcedure,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosStoredProcedure> commands,
            IMessenger messenger)
            : base(storedProcedure, context, parent, commands, messenger)
        {
        }

        public override string Description => "stored procedure";

        public override Task DeleteAsync() => Context.Scripts.DeleteStoredProcedureAsync(Script, default);
    }
}
