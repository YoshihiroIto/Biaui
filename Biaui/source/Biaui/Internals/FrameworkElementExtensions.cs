using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;

namespace Biaui.Internals
{
    internal static class FrameworkElementExtensions
    {
        internal const double BorderWidth = 1.0;
        internal const double BorderHalfWidth = BorderWidth * 0.5;
        
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

        internal static double MakeSlotMarkRadius(this FrameworkElement self, IBiaNodeItem nodeItem)
        {
            var slotMarkRadius = Constants.SlotMarkRadius(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));

            if (nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
            {
                var invScale = 1d / self.CalcCompositeRenderScale();
                slotMarkRadius *= invScale;
            }

            return slotMarkRadius;
        }

        internal static double MakeSlotMarkRadiusSq(this FrameworkElement self, IBiaNodeItem nodeItem)
        {
            var slotMarkRadiusSq = Constants.SlotMarkRadiusSq(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));

            if (nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
            {
                var invScale = 1d / self.CalcCompositeRenderScale();
                slotMarkRadiusSq *= invScale * invScale;
            }

            return slotMarkRadiusSq;
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