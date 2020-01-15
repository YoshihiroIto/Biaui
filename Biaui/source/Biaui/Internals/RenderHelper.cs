using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class RenderHelper
    {
        internal static void DrawPointCursor(this Visual visual, DrawingContext dc, in ImmutableVec2_double pos, bool isEnabled, bool isReadOnly)
        {
            var pointIn = Caches.GetPen(ByteColor.White, visual.RoundLayoutValue(PointCursorRadius - 2));
            var pointInIsReadOnly = Caches.GetPen(ByteColor.Gray, visual.RoundLayoutValue(PointCursorRadius - 2));
            var pointOut = Caches.GetPen(ByteColor.Black, visual.RoundLayoutValue(PointCursorRadius));
 
            var s = visual.RoundLayoutValue(1);

            var ob = pointOut;
            var ib = isEnabled == false || isReadOnly ? pointInIsReadOnly : pointIn;

            var p = new Point(pos.X, pos.Y);

            dc.DrawEllipse(ob.Brush, ob, p, s, s);
            dc.DrawEllipse(ib.Brush, ib, p, s, s);
        }

        private const double PointCursorRadius = 6;
    }
}