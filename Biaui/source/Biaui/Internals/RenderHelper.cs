using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class RenderHelper
    {
        internal static void DrawPointCursor(this Visual visual, DrawingContext dc, Point pos, bool isEnabled, bool isReadOnly)
        {
            var pointIn = Caches.GetPen(Colors.White, visual.RoundLayoutValue(PointCursorRadius - 2));
            var pointInIsReadOnly = Caches.GetPen(Colors.Gray, visual.RoundLayoutValue(PointCursorRadius - 2));
            var pointOut = Caches.GetPen(Colors.Black, visual.RoundLayoutValue(PointCursorRadius));
 
            var s = visual.RoundLayoutValue(1);

            var ob = pointOut;
            var ib = isEnabled == false || isReadOnly ? pointInIsReadOnly : pointIn;

            dc.DrawEllipse(ob.Brush, ob, pos, s, s);
            dc.DrawEllipse(ib.Brush, ib, pos, s, s);
        }

        private const double PointCursorRadius = 6;
    }
}