using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaCheckBoxViewModel : ViewModelBase
    {
        #region IsCheckedX

        private bool _IsCheckedX;

        public bool IsCheckedX
        {
            get => _IsCheckedX;
            set => SetProperty(ref _IsCheckedX, value);
        }

        #endregion

        #region IsCheckedY

        private bool _IsCheckedY;

        public bool IsCheckedY
        {
            get => _IsCheckedY;
            set => SetProperty(ref _IsCheckedY, value);
        }

        #endregion

        public BiaCheckBoxViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
        }
    }
}