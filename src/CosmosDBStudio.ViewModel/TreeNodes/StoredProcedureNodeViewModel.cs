using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;

namespace CosmosDBStudio.ViewModel.TreeNodes
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
