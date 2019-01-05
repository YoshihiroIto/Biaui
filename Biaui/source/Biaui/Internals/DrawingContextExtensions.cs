using System;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class DrawingContextExtensions
    {
        internal static void DrawBezier(this DrawingContext dc, Point[] bezierPoints, Pen pen)
        {
            var pf = new PathFigure
            {
                StartPoint = bezierPoints[0]
            };

            var bs = new BezierSegment(bezierPoints[1], bezierPoints[2], bezierPoints[3], true);

            pf.Segments.Add(bs);

            var curve = new PathGeometry();
            curve.Figures.Add(pf);

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