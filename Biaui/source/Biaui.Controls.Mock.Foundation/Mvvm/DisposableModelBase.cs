using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation.Mvvm
{
    public class DisposableModelBase : DisposableNotificationObject
    {
        public DisposableModelBase(IDisposableChecker disposableChecker)
            : base(disposableChecker)
        {
        }
    }
}