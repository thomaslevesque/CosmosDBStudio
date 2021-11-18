using EssentialMVVM;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public static class CommandExtensions
    {
        public static DelegateCommand WithParameter(this ICommand command, object parameter)
        {
            return new DelegateCommand(() => command.Execute(parameter), () => command.CanExecute(parameter));
        }
    }
}
