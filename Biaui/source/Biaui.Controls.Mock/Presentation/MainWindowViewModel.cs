using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IDisposableChecker disposableChecker)
            : base(disposableChecker)
        {
        }
    }
}