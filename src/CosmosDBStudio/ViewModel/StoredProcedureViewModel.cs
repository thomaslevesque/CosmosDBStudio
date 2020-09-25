using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureViewModel : ContainerScriptViewModel<CosmosStoredProcedure>
    {
        public StoredProcedureViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosStoredProcedure storedProcedure,
            IMessenger messenger)
            : base(container, parent, storedProcedure, messenger)
        {
        }
    }
}
