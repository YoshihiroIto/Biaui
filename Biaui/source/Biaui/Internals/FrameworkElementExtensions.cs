using System;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class FrameworkElementExtensions
    {
        internal static double RoundLayoutRenderWidth(this FrameworkElement self, bool isWithBorder)
        {
            if (isWithBorder)
            {
                return self.RoundLayoutValue(self.RenderSize.Width - BorderWidth);
            }
            else
            {
                return self.RoundLayoutValue(self.RenderSize.Width);
            }
        }


        internal static double RoundLayoutRenderHeight(this FrameworkElement self, bool isWithBorder)
        {
            if (isWithBorder)
            {
                return self.RoundLayoutValue(self.RenderSize.Height - BorderWidth);
            }
            else
            {
                return self.RoundLayoutValue(self.RenderSize.Height);
            }
        }

        internal static Rect RoundLayoutRenderRectangle(this FrameworkElement self, bool isWithBorder)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (isWithBorder)
            {
                return new Rect(
                    self.RoundLayoutValue(BorderHalfWidth),
                    self.RoundLayoutValue(BorderHalfWidth),
                    self.RoundLayoutRenderWidth(isWithBorder),
                    self.RoundLayoutRenderHeight(isWithBorder));
            }
            else
            {
                return new Rect(0, 0, self.RoundLayoutRenderWidth(isWithBorder),
                    self.RoundLayoutRenderHeight(isWithBorder));
            }
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        internal static double CalcCompositeRenderScale(this FrameworkElement self)
        {
            var scale = 1.0;

            var p = self as DependencyObject;

            do
            {
                if (p is FrameworkElement pp)
                {
                    switch (pp.RenderTransform)
                    {
                        case TransformGroup tg:
                        {
                            foreach (var c in tg.Children)
                            {
                                if (!(c is ScaleTransform sc))
                                    continue;

                                scale *= sc.ScaleX;
                            }

                            break;
                        }

                        case ScaleTransform st:
                            scale *= st.ScaleX;
                            break;
                    }
                }

                p = VisualTreeHelper.GetParent(p);
            } while (p != null);

            return scale;
        }

        internal static Pen GetBorderPen(this FrameworkElement self, ImmutableByteColor color)
            => Caches.GetPen(color, self.RoundLayoutValue(BorderWidth));

        public const double BorderWidth = 1.0;
        public const double BorderHalfWidth = BorderWidth * 0.5;

        internal static double RoundLayoutValue(this Visual visual, double value)
            => RoundLayoutValue(value, visual.PixelsPerDip());

        internal static ImmutableRect_double RoundLayoutRect(this Visual visual, in ImmutableRect_double rect)
        {
            var dpi = visual.PixelsPerDip();

            return new ImmutableRect_double(
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

                if (double.IsNaN(newValue) ||
                    double.IsInfinity(newValue) ||
                    NumberHelper.AreClose(newValue, double.MaxValue))
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

        internal static void SetMouseClipping(this FrameworkElement self)
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(self.ActualWidth + 1, self.ActualHeight + 1);
            var dp0 = self.PointToScreen(p0);
            var dp1 = self.PointToScreen(p1);
            var cr = new Win32Helper.RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
            Win32Helper.ClipCursor(ref cr);
        }

        internal static void ResetMouseClipping(this FrameworkElement self)
        {
            Win32Helper.ClipCursor(IntPtr.Zero);
        }

        internal static bool IsInActualSize(this FrameworkElement self, Point pos)
        {
            return pos.X >= 0 && pos.X <= self.ActualWidth &&
                   pos.Y >= 0 && pos.Y <= self.ActualHeight;
        }
    }
}