using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation;

public class BiaRadioButtonViewModel : ViewModelBase
{
    #region GroupA_X
        
    private bool _GroupA_X;
        
    public bool GroupA_X
    {
        get => _GroupA_X;
        set => SetProperty(ref _GroupA_X, value);
    }
        
    #endregion
        
    #region GroupA_Y
        
    private bool _GroupA_Y;
        
    public bool GroupA_Y
    {
        get => _GroupA_Y;
        set => SetProperty(ref _GroupA_Y, value);
    }
        
    #endregion
        
    #region GroupA_Z
        
    private bool _GroupA_Z;
        
    public bool GroupA_Z
    {
        get => _GroupA_Z;
        set => SetProperty(ref _GroupA_Z, value);
    }
        
    #endregion
        
    #region GroupB_X
        
    private bool _GroupB_X;
        
    public bool GroupB_X
    {
        get => _GroupB_X;
        set => SetProperty(ref _GroupB_X, value);
    }
        
    #endregion
        
    #region GroupB_Y
        
    private bool _GroupB_Y;
        
    public bool GroupB_Y
    {
        get => _GroupB_Y;
        set => SetProperty(ref _GroupB_Y, value);
    }
        
    #endregion
        
    #region GroupB_Z
        
    private bool _GroupB_Z;
        
    public bool GroupB_Z
    {
        get => _GroupB_Z;
        set => SetProperty(ref _GroupB_Z, value);
    }
        
    #endregion
        
    #region GroupC_X
        
    private bool _GroupC_X;
        
    public bool GroupC_X
    {
        get => _GroupC_X;
        set => SetProperty(ref _GroupC_X, value);
    }
        
    #endregion
        
    #region GroupC_Y
        
    private bool _GroupC_Y;
        
    public bool GroupC_Y
    {
        get => _GroupC_Y;
        set => SetProperty(ref _GroupC_Y, value);
    }
        
    #endregion
        
    #region GroupC_Z
        
    private bool _GroupC_Z;
        
    public bool GroupC_Z
    {
        get => _GroupC_Z;
        set => SetProperty(ref _GroupC_Z, value);
    }
        
    #endregion
        
    public BiaRadioButtonViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
    {
    }
}