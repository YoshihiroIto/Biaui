using System;
using System.Windows;
using System.Windows.Media;
using static System.Double;

namespace Biaui.Internals
{
    internal static class FrameworkElementHelper
    {
        internal static double RoundLayoutValue(this Visual visual, double value)
            => RoundLayoutValue(value, visual.PixelsPerDip());

        internal static ImmutableRect RoundLayoutRect(this Visual visual, in ImmutableRect rect)
        {
            var dpi = visual.PixelsPerDip();

            return new ImmutableRect(
                RoundLayoutValue(rect.X, dpi),
                RoundLayoutValue(rect.Y, dpi),
                RoundLayoutValue(rect.Width, dpi),
                RoundLayoutValue(rect.Height, dpi));
        }

        internal static Rect RoundLayoutRect(this Visual visual, double x, double y, double w, double h)
        {
            var dpi = visual.PixelsPerDip();

            return new Rect(
                RoundLayoutValue(x, dpi),
                RoundLayoutValue(y, dpi),
                RoundLayoutValue(w, dpi),
                RoundLayoutValue(h, dpi));
        }

        private static double RoundLayoutValue(double value, double dpiScale)
        {
            double newValue;

            if (NumberHelper.AreClose(dpiScale, 1.0) == false)
            {
                newValue = Math.Round(value * dpiScale) / dpiScale;

                if (IsNaN(newValue) ||
                    IsInfinity(newValue) ||
                    NumberHelper.AreClose(newValue, MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = value;
            }

            return newValue;
        }
    }
}