using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class WpfHelper
    {
        internal static double PixelsPerDip(this Visual visual)
            => VisualTreeHelper.GetDpi(visual).DpiScaleX;
    }
}