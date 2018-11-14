using System;
using System.Runtime.InteropServices;

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

                var hwnd = GetDesktopWindow();
                var hdc = GetWindowDC(hwnd);

                try
                {
                    _pixelsPerDip = (double) GetDeviceCaps(hdc, 90) / 96;
                }
                finally
                {
                    ReleaseDC(hwnd, hdc);
                }

                return _pixelsPerDip;
            }
        }

        private static double _pixelsPerDip;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    }
}