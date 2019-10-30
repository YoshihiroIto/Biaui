using System;
using System.Windows;
using System.Windows.Media;
using Jewelry.Collections;

namespace Biaui.Internals
{
    internal static class DrawingContextExtensions
    {
        internal static void DrawBezier(this DrawingContext dc, ImmutableVec2[] bezierPoints, Pen pen)
        {
            DrawBezier(dc, bezierPoints[0], bezierPoints[1], bezierPoints[2], bezierPoints[3], pen);
        }

        private static readonly LruCache<int, PathGeometry> _bezierCache = new LruCache<int, PathGeometry>(10000, false);

        internal static void DrawBezier(
            this DrawingContext dc,
            in ImmutableVec2 pos1,
            in ImmutableVec2 pos1C,
            in ImmutableVec2 pos2C,
            in ImmutableVec2 pos2,
            Pen pen)
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

            if (_bezierCache.TryGetValue(hashCode, out var curve) == false)
            {
                var pf = new PathFigure
                {
                    StartPoint = new Point(pos1.X, pos1.Y)
                };

                var bs = new BezierSegment(
                    new Point(pos1C.X, pos1C.Y),
                    new Point(pos2C.X, pos2C.Y),
                    new Point(pos2.X, pos2.Y),
                    true);
                bs.Freeze();

                pf.Segments.Add(bs);
                pf.Freeze();

                curve = new PathGeometry();
                curve.Figures.Add(pf);
                curve.Freeze();

                _bezierCache.Add(hashCode, curve);
            }

            dc.DrawGeometry(null, pen, curve);
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