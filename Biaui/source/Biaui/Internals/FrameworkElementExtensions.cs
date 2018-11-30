using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class FrameworkElementExtensions
    {
        internal static double RoundLayoutActualWidth(this FrameworkElement self, bool isWithBorder = false)
        {
            if (isWithBorder)
            {
                return FrameworkElementHelper.RoundLayoutValue(self.ActualWidth - BorderWidth);
            }
            else
            {
                return FrameworkElementHelper.RoundLayoutValue(self.ActualWidth);
            }
        }


        internal static double RoundLayoutActualHeight(this FrameworkElement self, bool isWithBorder = false)
        {
            if (isWithBorder)
            {
                return FrameworkElementHelper.RoundLayoutValue(self.ActualHeight - BorderWidth);
            }
            else
            {
                return FrameworkElementHelper.RoundLayoutValue(self.ActualHeight);
            }
        }

        internal static Rect RoundLayoutActualRectangle(this FrameworkElement self, bool isWithBorder = false)
        {
            if (isWithBorder)
            {
                return new Rect(
                    RoundLayoutBorderWidth,
                    RoundLayoutBorderWidth,
                    self.RoundLayoutActualWidth() - RoundLayoutBorderWidth * 2,
                    self.RoundLayoutActualHeight() - RoundLayoutBorderWidth * 2);
            }
            else
            {
                return new Rect(0, 0, self.RoundLayoutActualWidth(), self.RoundLayoutActualHeight());
            }
        }

        internal static Pen GetBorderPen(this FrameworkElement self, Color color)
            => Caches.GetBorderPen(color, FrameworkElementHelper.RoundLayoutValue(BorderWidth));

        public const double BorderWidth = 1.0;

        private static readonly double RoundLayoutBorderWidth =
            FrameworkElementHelper.RoundLayoutValue(BorderWidth * 0.5);
    }
}