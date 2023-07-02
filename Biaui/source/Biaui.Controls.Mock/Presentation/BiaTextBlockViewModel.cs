using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation;

public class BiaTextBlockViewModel : ViewModelBase
{
    #region TextA

    private string _TextA = "";

    public string TextA
    {
        get => _TextA;
        set => SetProperty(ref _TextA, value);
    }

    #endregion

    #region TextB

    private string _TextB = "";

    public string TextB
    {
        get => _TextB;
        set => SetProperty(ref _TextB, value);
    }

    #endregion

    #region TextC

    private string _TextC = "";

    public string TextC
    {
        get => _TextC;
        set => SetProperty(ref _TextC, value);
    }

    #endregion

    public BiaTextBlockViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
    {
        TextA = "A";
        TextB = "B";
        TextC = "C";
    }
}