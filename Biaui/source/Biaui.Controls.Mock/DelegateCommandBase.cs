using System;
using System.Windows.Input;

namespace Biaui.Controls.Mock;

public interface IDelegateCommand : ICommand
{
    IDelegateCommand Setup(Action execute);
    IDelegateCommand Setup(Action execute, Func<bool> canExecute);
}

public interface IDelegateCommand<T> : ICommand
{
    IDelegateCommand<T> Setup(Action<T?> execute);
    IDelegateCommand<T> Setup(Action<T?> execute, Func<T?, bool> canExecute);
}

public abstract class DelegateCommandBase : IDelegateCommand
{
    private Action? _execute;
    private Func<bool>? _canExecute;

    public IDelegateCommand Setup(Action execute)
    {
        _execute = execute;
        return this;
    }

    public IDelegateCommand Setup(Action execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
        return this;
    }

    public abstract event EventHandler? CanExecuteChanged;

    bool ICommand.CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    void ICommand.Execute(object? parameter) => _execute?.Invoke();
}

public abstract class DelegateCommandBase<T> : IDelegateCommand<T>
{
    private Action<T?>? _execute;
    private Func<T?, bool>? _canExecute;

    public IDelegateCommand<T> Setup(Action<T?> execute)
    {
        _execute = execute;
        return this;
    }

    public IDelegateCommand<T> Setup(Action<T?> execute, Func<T?, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
        return this;
    }

    public abstract event EventHandler? CanExecuteChanged;

    bool ICommand.CanExecute(object? parameter) => _canExecute?.Invoke((T?) parameter) ?? true;
    
    void ICommand.Execute(object? parameter) => _execute?.Invoke((T?) parameter);
}