using System.Windows.Input;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.Commands
{
    public static class CommandExtensions
    {
        public static DelegateCommand WithParameter(this ICommand command, object parameter)
        {
            return new DelegateCommand(() => command.Execute(parameter), () => command.CanExecute(parameter));
        }
    }
}
