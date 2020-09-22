using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionsFolderViewModel : ContainerScriptFolderViewModel
    {
        public UserDefinedFunctionsFolderViewModel(
            ContainerViewModel container,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory)
            : base(container, "User-defined functions", accountManager, viewModelFactory)
        {
        }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var database = Container.Database;
            var account = database.Account;
            var functions = await AccountManager.GetUserDefinedFunctionsAsync(account.Id, database.Id, Container.Id);
            return functions.Select(udf => ViewModelFactory.CreateUserDefinedFunction(Container, this, udf));
        }
    }
}
