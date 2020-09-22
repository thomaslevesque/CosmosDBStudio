using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureViewModel : ContainerScriptViewModel<CosmosStoredProcedure>
    {
        public StoredProcedureViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosStoredProcedure storedProcedure)
            : base(container, parent, storedProcedure)
        {
        }
    }
}
