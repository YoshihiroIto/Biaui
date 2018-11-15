using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaCheckBoxViewModel : ViewModelBase
    {
        public BiaCheckBoxViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
        }
    }
}