namespace Biaui.Internals
{
    internal static class WpfHelper
    {
        internal static double PixelsPerDip
        {
            get
            {
                if (NumberHelper.AreCloseZero(_pixelsPerDip) == false)
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
    }
}