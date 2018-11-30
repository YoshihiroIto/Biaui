using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace Biaui.Internals
{
    internal static class WpfHelper
    {
        public static double PixelsPerDip
        {
            get
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pixelsPerDip != default(double))
                    return _pixelsPerDip;

                var hwnd = Win32Helper.GetDesktopWindow();
                var hdc = Win32Helper.GetWindowDC(hwnd);

                try
                {
                    _pixelsPerDip = (double) Win32Helper.GetDeviceCaps(hdc, 90) / 96;
                }
                finally
                {
                    Win32Helper.ReleaseDC(hwnd, hdc);
                }

                return _pixelsPerDip;
            }
        }

        private static double _pixelsPerDip;

        public static IntPtr GetHwnd(Popup popup)
        {
            var source = (HwndSource) PresentationSource.FromVisual(popup.Child);

            return source?.Handle ?? IntPtr.Zero;
        }
    }
}