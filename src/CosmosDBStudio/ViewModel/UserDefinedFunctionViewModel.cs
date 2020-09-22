using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionViewModel : ContainerScriptViewModel<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosUserDefinedFunction udf)
            : base(container, parent, udf)
        {
        }
    }
}
