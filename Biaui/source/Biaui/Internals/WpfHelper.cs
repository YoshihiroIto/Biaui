using System.Windows;

namespace Biaui.Internals
{
    public static class WpfHelper
    {
        public static double PixelsPerDip
        {
            get
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pixelsPerDip != default(double))
                    return _pixelsPerDip;

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow == null)
                {
                    _pixelsPerDip = 1.0f;
                    return _pixelsPerDip;
                }

                var v = PresentationSource.FromVisual(mainWindow);
                if (v?.CompositionTarget == null)
                {
                    _pixelsPerDip = 1.0f;
                    return _pixelsPerDip;
                }

                _pixelsPerDip = (float) v.CompositionTarget.TransformToDevice.M11;
                return _pixelsPerDip;
            }
        }

        private static double _pixelsPerDip;
    }
}