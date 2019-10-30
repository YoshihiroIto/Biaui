using System;
using System.Windows;
using System.Windows.Media;
using Jewelry.Collections;

namespace Biaui.Internals
{
    internal static class DrawingContextExtensions
    {
        internal static void DrawBezier(this DrawingContext dc, Point[] bezierPoints, Pen pen)
        {
            DrawBezier(dc, bezierPoints[0], bezierPoints[1], bezierPoints[2], bezierPoints[3], pen);
        }

        private static readonly LruCache<int, PathGeometry> _bezierCache = new LruCache<int, PathGeometry>(10000, false);

        internal static void DrawBezier(this DrawingContext dc, Point pos1, Point pos1C, Point pos2C, Point pos2, Pen pen)
        {
            var hashCode = HashCodeMaker.Make(
                pos1.X,
                pos1.Y,
                pos1C.X,
                pos1C.Y,
                pos2C.X,
                pos2C.Y,
                pos2.X,
                pos2.Y);

            var c = _bezierCache.GetOrAdd(
                hashCode,
                _ =>
                {
                    var pf = new PathFigure
                    {
                        StartPoint = pos1
                    };

                    var bs = new BezierSegment(pos1C, pos2C, pos2, true);
                    bs.Freeze();

                    pf.Segments.Add(bs);
                    pf.Freeze();

                    var curve = new PathGeometry();
                    curve.Figures.Add(pf);
                    curve.Freeze();

                    return curve;
                });

            dc.DrawGeometry(null, pen, c);
        }

        internal static void DrawCircle(this DrawingContext dc, Brush brush, Pen pen, Point pos, double radius)
        {
            var geom = new StreamGeometry
            {
                FillRule = FillRule.Nonzero
            };

            var ctx = geom.Open();
            {
                ctx.DrawEllipse(
                    pos,
                    radius,
                    radius,
                    true,
                    true);
            }
            ((IDisposable) ctx).Dispose();

            dc.DrawGeometry(brush, pen, geom);
        }
    }
}