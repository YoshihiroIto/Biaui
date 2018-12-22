using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class RenderHelper
    {
        internal static void DrawPointCursor(DrawingContext dc, Point pos, bool isEnabled, bool isReadOnly)
        {
            var s = FrameworkElementHelper.RoundLayoutValue(1);

            var ob = PointOut;
            var ib = isEnabled == false || isReadOnly ? PointInIsReadOnly : PointIn;

            dc.DrawEllipse(ob.Brush, ob, pos, s, s);
            dc.DrawEllipse(ib.Brush, ib, pos, s, s);
        }

        private static readonly Pen PointIn;
        private static readonly Pen PointInIsReadOnly;
        private static readonly Pen PointOut;

        private const double PointCursorRadius = 6;

        static RenderHelper()
        {
            PointIn = new Pen(Brushes.White, FrameworkElementHelper.RoundLayoutValue(PointCursorRadius - 2));
            PointIn.Freeze();

            PointInIsReadOnly = new Pen(Brushes.Gray, FrameworkElementHelper.RoundLayoutValue(PointCursorRadius - 2));
            PointInIsReadOnly.Freeze();

            PointOut = new Pen(Brushes.Black, FrameworkElementHelper.RoundLayoutValue(PointCursorRadius));
            PointOut.Freeze();
        }
    }
}