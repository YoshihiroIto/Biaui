using Biaui.Controls.Mock.Foundation;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using SimpleInjector;

namespace Biaui.Controls.Mock.Presentation;

public class MainWindowViewModel : ViewModelBase
{
    public BiaNumberEditorViewModel BiaNumberEditorViewModel { get; }
    public BiaColorPickerViewModel BiaColorPickerViewModel { get; }
    public BiaButtonViewModel BiaButtonViewModel { get; }
    public BiaToggleButtonViewModel BiaToggleButtonViewModel { get; }
    public BiaCheckBoxViewModel BiaCheckBoxViewModel { get; }
    public BiaRadioButtonViewModel BiaRadioButtonViewModel { get; }
    public BiaTextBoxViewModel BiaTextBoxViewModel { get; }
    public BiaComboBoxViewModel BiaComboBoxViewModel { get; }
    public BiaTextBlockViewModel BiaTextBlockViewModel { get; }

    public MainWindowViewModel(Container dic, IDisposableChecker disposableChecker)
        : base(disposableChecker)
    {
        BiaNumberEditorViewModel = dic.GetInstance<BiaNumberEditorViewModel>().AddTo(Trashes);
        BiaColorPickerViewModel = dic.GetInstance<BiaColorPickerViewModel>().AddTo(Trashes);
        BiaButtonViewModel = dic.GetInstance<BiaButtonViewModel>().AddTo(Trashes);
        BiaToggleButtonViewModel = dic.GetInstance<BiaToggleButtonViewModel>().AddTo(Trashes);
        BiaCheckBoxViewModel = dic.GetInstance<BiaCheckBoxViewModel>().AddTo(Trashes);
        BiaRadioButtonViewModel = dic.GetInstance<BiaRadioButtonViewModel>().AddTo(Trashes);
        BiaTextBoxViewModel = dic.GetInstance<BiaTextBoxViewModel>().AddTo(Trashes);
        BiaComboBoxViewModel = dic.GetInstance<BiaComboBoxViewModel>().AddTo(Trashes);
        BiaTextBlockViewModel = dic.GetInstance<BiaTextBlockViewModel>().AddTo(Trashes);
    }
}