using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation;

public class BiaColorPickerViewModel : ViewModelBase
{
    #region Red
        
    private double _Red;
        
    public double Red
    {
        get => _Red;
        set => SetProperty(ref _Red, value);
    }
        
    #endregion
        
    #region Green
        
    private double _Green;
        
    public double Green
    {
        get => _Green;
        set => SetProperty(ref _Green, value);
    }
        
    #endregion
        
    #region Blue
        
    private double _Blue;
        
    public double Blue
    {
        get => _Blue;
        set => SetProperty(ref _Blue, value);
    }
        
    #endregion
        
    #region Hue
        
    private double _Hue;
        
    public double Hue
    {
        get => _Hue;
        set => SetProperty(ref _Hue, value);
    }
        
    #endregion
        
    #region Saturation
        
    private double _Saturation;
        
    public double Saturation
    {
        get => _Saturation;
        set => SetProperty(ref _Saturation, value);
    }
        
    #endregion
        
    #region Value
        
    private double _Value;
        
    public double Value
    {
        get => _Value;
        set => SetProperty(ref _Value, value);
    }
        
    #endregion

    public BiaColorPickerViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
    {
        Red = 1;
        Green = 0.5;
        Blue = 0.25;
    }
}