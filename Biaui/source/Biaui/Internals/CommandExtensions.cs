using System.Windows.Input;

namespace Biaui.Internals;

internal static class CommandExtensions
{
    internal static void ExecuteIfCan(this ICommand command, object? parameter)
    {
        if (command.CanExecute(parameter))
            command.Execute(parameter);
    }
}
