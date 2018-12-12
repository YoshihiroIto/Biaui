using System;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class FrameworkElementExtensions
    {
        internal static double RoundLayoutActualWidth(this FrameworkElement self, bool isWithBorder)
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


        internal static double RoundLayoutActualHeight(this FrameworkElement self, bool isWithBorder)
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

        internal static Rect RoundLayoutActualRectangle(this FrameworkElement self, bool isWithBorder)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (isWithBorder)
            {
                return new Rect(
                    RoundLayoutBorderWidth,
                    RoundLayoutBorderWidth,
                    self.RoundLayoutActualWidth(isWithBorder),
                    self.RoundLayoutActualHeight(isWithBorder));
            }
            else
            {
                return new Rect(0, 0, self.RoundLayoutActualWidth(isWithBorder),
                    self.RoundLayoutActualHeight(isWithBorder));
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
                                var sc = c as ScaleTransform;
                                if (sc == null)
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

        internal static Pen GetBorderPen(this FrameworkElement self, Color color)
            => Caches.GetBorderPen(color, FrameworkElementHelper.RoundLayoutValue(BorderWidth));

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

        internal const double BorderWidth = 1.0;

        private static readonly double RoundLayoutBorderWidth =
            FrameworkElementHelper.RoundLayoutValue(BorderWidth * 0.5);
    }
}