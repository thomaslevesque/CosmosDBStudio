using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionViewModel : ContainerScriptViewModel<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosUserDefinedFunction udf,
            IMessenger messenger)
            : base(container, parent, udf, messenger)
        {
        }
    }
}
