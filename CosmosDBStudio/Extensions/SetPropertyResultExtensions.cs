using EssentialMVVM;

namespace CosmosDBStudio.Extensions
{
    public static class SetPropertyResultExtensions
    {
        public static SetPropertyResult AndRefreshCanExecute(this SetPropertyResult result, DelegateCommandBase? command)
        {
            if (result)
                command?.RaiseCanExecuteChanged();
            return result;
        }
    }
}
