using Biaui.Controls.Mock.Foundation;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using SimpleInjector;

namespace Biaui.Controls.Mock.Presentation
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region BiaNumberEditorViewModel

        private BiaNumberEditorViewModel _BiaNumberEditorViewModel;

        public BiaNumberEditorViewModel BiaNumberEditorViewModel
        {
            get => _BiaNumberEditorViewModel;
            set => SetProperty(ref _BiaNumberEditorViewModel, value);
        }

        #endregion

        #region BiaColorPickerViewModel
        
        private BiaColorPickerViewModel _BiaColorPickerViewModel;
        
        public BiaColorPickerViewModel BiaColorPickerViewModel
        {
            get => _BiaColorPickerViewModel;
            set => SetProperty(ref _BiaColorPickerViewModel, value);
        }
        
        #endregion

        public MainWindowViewModel(Container dic, IDisposableChecker disposableChecker)
            : base(disposableChecker)
        {
            BiaNumberEditorViewModel = dic.GetInstance<BiaNumberEditorViewModel>().AddTo(Trashes);
            BiaColorPickerViewModel = dic.GetInstance<BiaColorPickerViewModel>().AddTo(Trashes);
        }
    }
}