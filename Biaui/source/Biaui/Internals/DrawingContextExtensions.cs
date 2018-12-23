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
    }
}