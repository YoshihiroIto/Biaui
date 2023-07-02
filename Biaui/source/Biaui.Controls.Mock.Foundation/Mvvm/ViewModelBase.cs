using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation.Mvvm;

public class ViewModelBase : DisposableNotificationObject
{
    public ViewModelBase(IDisposableChecker disposableChecker)
        : base(disposableChecker)
    {
    }
}