using System.Windows.Input;

namespace CosmosDBStudio.Extensions
{
    public static class CommandExtensions
    {
        public static void TryExecute(this ICommand? command, object? parameter)
        {
            if (command is null)
                return;
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
