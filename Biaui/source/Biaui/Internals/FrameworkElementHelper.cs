using System;
using System.Windows;
using static System.Double;

namespace Biaui.Internals
{
    internal static class FrameworkElementHelper
    {
        internal static double RoundLayoutValue(double value)
            => RoundLayoutValue(value, WpfHelper.PixelsPerDip);

        internal static ImmutableRect RoundLayoutRect(in ImmutableRect rect)
            => new ImmutableRect(
                RoundLayoutValue(rect.X, WpfHelper.PixelsPerDip),
                RoundLayoutValue(rect.Y, WpfHelper.PixelsPerDip),
                RoundLayoutValue(rect.Width, WpfHelper.PixelsPerDip),
                RoundLayoutValue(rect.Height, WpfHelper.PixelsPerDip));

        internal static Rect RoundLayoutRect(double x, double y, double w, double h)
            => new Rect(
                RoundLayoutValue(x, WpfHelper.PixelsPerDip),
                RoundLayoutValue(y, WpfHelper.PixelsPerDip),
                RoundLayoutValue(w, WpfHelper.PixelsPerDip),
                RoundLayoutValue(h, WpfHelper.PixelsPerDip));

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
                newValue = Math.Round(value);
            }

            return newValue;
        }
    }
}