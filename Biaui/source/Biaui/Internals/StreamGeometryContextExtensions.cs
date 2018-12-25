using System;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class StreamGeometryContextExtensions
    {
        // https://stackoverflow.com/questions/2979834/how-to-draw-a-full-ellipse-in-a-streamgeometry-in-wpf
        private static readonly double ControlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;

        internal static void DrawEllipse(this StreamGeometryContext ctx,
            Point pos, double radiusX, double radiusY, bool isStroked)
        {
            var x0 = pos.X - radiusX;
            var x1 = pos.X - radiusX * ControlPointRatio;
            var x2 = pos.X;
            var x3 = pos.X + radiusX * ControlPointRatio;
            var x4 = pos.X + radiusX;

            var y0 = pos.Y - radiusY;
            var y1 = pos.Y - radiusY * ControlPointRatio;
            var y2 = pos.Y;
            var y3 = pos.Y + radiusY * ControlPointRatio;
            var y4 = pos.Y + radiusY;

            ctx.BeginFigure(new Point(x2, y0), true, true);
            ctx.BezierTo(new Point(x3, y0), new Point(x4, y1), new Point(x4, y2), isStroked, true);
            ctx.BezierTo(new Point(x4, y3), new Point(x3, y4), new Point(x2, y4), isStroked, true);
            ctx.BezierTo(new Point(x1, y4), new Point(x0, y3), new Point(x0, y2), isStroked, true);
            ctx.BezierTo(new Point(x0, y1), new Point(x1, y0), new Point(x2, y0), isStroked, true);
        }

        internal static void DrawTriangle(this StreamGeometryContext ctx,
            Point pos1, Point pos2, Point pos3, bool isStroked, bool isClosed)
        {
            ctx.BeginFigure(pos1, true, isClosed);
            ctx.LineTo(pos2, isStroked, false);
            ctx.LineTo(pos3, isStroked, false);
        }
    }
}