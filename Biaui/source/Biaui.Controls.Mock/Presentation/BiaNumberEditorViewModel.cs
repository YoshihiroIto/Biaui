using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaNumberEditorViewModel : ViewModelBase
    {
        #region MinMaxValue

        private double _MinMaxValue;

        public double MinMaxValue
        {
            get => _MinMaxValue;
            set => SetProperty(ref _MinMaxValue, value);
        }

        #endregion

        #region SpinValue

        private double _spinValue;

        public double SpinValue
        {
            get => _spinValue;
            set => SetProperty(ref _spinValue, value);
        }

        #endregion

        #region TextInputValue
        
        private double _TextInputValue;
        
        public double TextInputValue
        {
            get => _TextInputValue;
            set => SetProperty(ref _TextInputValue, value);
        }
        
        #endregion

        #region SliderValueSimple
        
        private double _SliderValueSimple;
        
        public double SliderValueSimple
        {
            get => _SliderValueSimple;
            set => SetProperty(ref _SliderValueSimple, value);
        }
        
        #endregion

        #region SliderValueWideRange
        
        private double _SliderValueWideRange;
        
        public double SliderValueWideRange
        {
            get => _SliderValueWideRange;
            set => SetProperty(ref _SliderValueWideRange, value);
        }
        
        #endregion

        public BiaNumberEditorViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
            MinMaxValue = 50.0;
            SpinValue = 50.0;
            TextInputValue = 50.0;
            SliderValueSimple = 50.0;
            SliderValueWideRange = 50.0;
        }
    }
}