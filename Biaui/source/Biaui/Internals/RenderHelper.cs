using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals;

internal static class RenderHelper
{
    internal static void DrawPointCursor(this Visual visual, in LayoutRounder rounder, DrawingContext dc, in ImmutableVec2_double pos, bool isEnabled, bool isReadOnly)
    {
        var pointIn = Caches.GetPen(ByteColor.White, rounder.RoundLayoutValue(PointCursorRadius - 2d));
        var pointInIsReadOnly = Caches.GetPen(ByteColor.Gray, rounder.RoundLayoutValue(PointCursorRadius - 2d));
        var pointOut = Caches.GetPen(ByteColor.Black, rounder.RoundLayoutValue(PointCursorRadius));

        var s = rounder.RoundLayoutValue(1);

        var ob = pointOut;
        var ib = isEnabled == false || isReadOnly ? pointInIsReadOnly : pointIn;

        var p = new Point(pos.X, pos.Y);

        dc.DrawEllipse(ob.Brush, ob, p, s, s);
        dc.DrawEllipse(ib.Brush, ib, p, s, s);
    }

    private const double PointCursorRadius = 6d;
}
