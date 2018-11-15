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

        #region BiaButtonViewModel
        
        private BiaButtonViewModel _BiaButtonViewModel;
        
        public BiaButtonViewModel BiaButtonViewModel
        {
            get => _BiaButtonViewModel;
            set => SetProperty(ref _BiaButtonViewModel, value);
        }
        
        #endregion

        #region BiaToggleButtonViewModel
        
        private BiaToggleButtonViewModel _BiaToggleButtonViewModel;
        
        public BiaToggleButtonViewModel BiaToggleButtonViewModel
        {
            get => _BiaToggleButtonViewModel;
            set => SetProperty(ref _BiaToggleButtonViewModel, value);
        }
        
        #endregion

        #region BiaCheckBoxViewModel
        
        private BiaCheckBoxViewModel _BiaCheckBoxViewModel;
        
        public BiaCheckBoxViewModel BiaCheckBoxViewModel
        {
            get => _BiaCheckBoxViewModel;
            set => SetProperty(ref _BiaCheckBoxViewModel, value);
        }
        
        #endregion

        public MainWindowViewModel(Container dic, IDisposableChecker disposableChecker)
            : base(disposableChecker)
        {
            BiaNumberEditorViewModel = dic.GetInstance<BiaNumberEditorViewModel>().AddTo(Trashes);
            BiaColorPickerViewModel = dic.GetInstance<BiaColorPickerViewModel>().AddTo(Trashes);
            BiaButtonViewModel = dic.GetInstance<BiaButtonViewModel>().AddTo(Trashes);
            BiaToggleButtonViewModel = dic.GetInstance<BiaToggleButtonViewModel>().AddTo(Trashes);
            BiaCheckBoxViewModel = dic.GetInstance<BiaCheckBoxViewModel>().AddTo(Trashes);
        }
    }
}