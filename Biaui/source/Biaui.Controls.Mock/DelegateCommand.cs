using System;
using System.Windows.Input;

namespace Biaui.Controls.Mock
{
    public class DelegateCommand : DelegateCommandBase
    {
        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class DelegateCommand<T> : DelegateCommandBase<T>
    {
        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}