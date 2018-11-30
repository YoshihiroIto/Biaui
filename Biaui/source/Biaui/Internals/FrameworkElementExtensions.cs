using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class FrameworkElementExtensions
    {
        internal static double RoundLayoutActualWidth(this FrameworkElement self)
            => FrameworkElementHelper.RoundLayoutValue(self.ActualWidth);

        internal static double RoundLayoutActualHeight(this FrameworkElement self)
            => FrameworkElementHelper.RoundLayoutValue(self.ActualHeight);

        internal static Rect RoundLayoutActualRectangle(this FrameworkElement self)
            => new Rect(0, 0, self.RoundLayoutActualWidth(), self.RoundLayoutActualHeight());

        internal static Pen GetBorderPen(this FrameworkElement self, Color color)
            => Caches.GetBorderPen(color, FrameworkElementHelper.RoundLayoutValue(BorderWidth));

        private const double BorderWidth = 1.0;

    }
}