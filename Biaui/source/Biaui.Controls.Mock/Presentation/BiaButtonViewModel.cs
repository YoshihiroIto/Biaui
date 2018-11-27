using System.Windows.Input;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaButtonViewModel : ViewModelBase
    {
        #region CommandA
        
        private ICommand _CommandA;
        
        public ICommand CommandA
        {
            get => _CommandA;
            set => SetProperty(ref _CommandA, value);
        }
        
        #endregion

        #region CountA
        
        private int _CountA;
        
        public int CountA
        {
            get => _CountA;
            set => SetProperty(ref _CountA, value);
        }
        
        #endregion

        #region CommandB
        
        private ICommand _CommandB;
        
        public ICommand CommandB
        {
            get => _CommandB;
            set => SetProperty(ref _CommandB, value);
        }
        
        #endregion
        
        #region ResultB
        
        private string _ResultB;
        
        public string ResultB
        {
            get => _ResultB;
            set => SetProperty(ref _ResultB, value);
        }
        
        #endregion

        #region CommandC
        
        private ICommand _CommandC;
        
        public ICommand CommandC
        {
            get => _CommandC;
            set => SetProperty(ref _CommandC, value);
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
            _CommandA = new DelegateCommand().Setup(() => ++ CountA);
            _CommandB = new DelegateCommand<string>().Setup(p => ResultB = p);
            _CommandC = new DelegateCommand().Setup(() => ++ CountC, () => CountC < 3);
        }
    }
}