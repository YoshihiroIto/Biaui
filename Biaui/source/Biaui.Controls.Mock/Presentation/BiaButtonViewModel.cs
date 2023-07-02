using System.Windows.Input;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using System;

namespace Biaui.Controls.Mock.Presentation;

public class BiaButtonViewModel : ViewModelBase
{
    public ICommand CommandA { get; }
    public ICommand CommandB { get; }
    public ICommand CommandC { get; }
    
    #region CountA
        
    private int _CountA;
        
    public int CountA
    {
        get => _CountA;
        set => SetProperty(ref _CountA, value);
    }
        
    #endregion
        
    #region ResultB
        
    private string _ResultB = "";
        
    public string ResultB
    {
        get => _ResultB;
        set => SetProperty(ref _ResultB, value);
    }
        
    #endregion
        
    #region CountC
        
    private int _CountC;
        
    public int CountC
    {
        get => _CountC;
        set => SetProperty(ref _CountC, value);
    }
        
    #endregion

    public BiaButtonViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
    {
        CommandA = new DelegateCommand().Setup(() => ++ CountA);
        CommandB = new DelegateCommand<string>().Setup(p => ResultB = p ?? throw new InvalidOperationException());
        CommandC = new DelegateCommand().Setup(() => ++ CountC, () => CountC < 3);
    }
}