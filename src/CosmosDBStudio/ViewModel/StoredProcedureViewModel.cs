using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureViewModel : ContainerScriptViewModel<CosmosStoredProcedure>
    {
        public StoredProcedureViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosStoredProcedure storedProcedure,
            ScriptCommands<CosmosStoredProcedure> commands,
            IMessenger messenger)
            : base(container, parent, storedProcedure, commands, messenger)
        {
        }

        public override string Description => "stored procedure";

        public override Task DeleteAsync(ICosmosAccountManager accountManager)
        {
            return accountManager.DeleteStoredProcedureAsync(
                Container.Database.Account.Id,
                Container.Database.Id,
                Container.Id,
                Script);
        }
    }
}
