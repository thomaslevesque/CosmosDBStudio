using System.Windows.Input;

namespace CosmosDBStudio.Avalonia.Extensions;

public static class CommandExtensions
{
    public static bool TryExecute(this ICommand? command, object? parameter)
    {
        if (command is null)
            return false;

        if (command.CanExecute(parameter))
        {
            command.Execute(parameter);
            return true;
        }

        return false;
    }
}