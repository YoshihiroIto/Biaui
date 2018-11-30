using System;
using System.Windows;
using static System.Double;

namespace Biaui.Internals
{
    internal static class FrameworkElementHelper
    {
        internal static double RoundLayoutValue(double value)
            => RoundLayoutValue(value, WpfHelper.PixelsPerDip);

        internal static Rect RoundLayoutRect(double x, double y, double w, double h)
            => new Rect(
                RoundLayoutValue(x, WpfHelper.PixelsPerDip),
                RoundLayoutValue(y, WpfHelper.PixelsPerDip),
                RoundLayoutValue(w, WpfHelper.PixelsPerDip),
                RoundLayoutValue(h, WpfHelper.PixelsPerDip));

        private static double RoundLayoutValue(double value, double dpiScale)
        {
            double newValue;

            if (AreClose(dpiScale, 1.0) == false)
            {
                newValue = Math.Round(value * dpiScale) / dpiScale;

                if (IsNaN(newValue) ||
                    IsInfinity(newValue) ||
                    AreClose(newValue, MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = Math.Round(value);
            }

            return newValue;

            ///////////////////////////////////////////////////////////////////////////////////////////////
            bool AreClose(double value1, double value2)
            {
                const double DBL_EPSILON = 2.2204460492503131e-016;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value1 == value2)
                    return true;

                var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
                var delta = value1 - value2;

                return (-eps < delta) && (eps > delta);
            }
        }
    }
}