using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Internals;

internal static class StreamGeometryContextExtensions
{
    // https://stackoverflow.com/questions/2979834/how-to-draw-a-full-ellipse-in-a-streamgeometry-in-wpf
    private static readonly double ControlPointRatio = (Math.Sqrt(2d) - 1) * 4d / 3d;

    internal static void DrawEllipse(this StreamGeometryContext ctx,
        Point center, double radiusX, double radiusY, bool isFilled, bool isStroked)
    {
        var x0 = center.X - radiusX;
        var x1 = center.X - radiusX * ControlPointRatio;
        var x2 = center.X;
        var x3 = center.X + radiusX * ControlPointRatio;
        var x4 = center.X + radiusX;

        var y0 = center.Y - radiusY;
        var y1 = center.Y - radiusY * ControlPointRatio;
        var y2 = center.Y;
        var y3 = center.Y + radiusY * ControlPointRatio;
        var y4 = center.Y + radiusY;

        ctx.BeginFigure(new Point(x2, y0), isFilled, true);
        ctx.BezierTo(new Point(x3, y0), new Point(x4, y1), new Point(x4, y2), isStroked, true);
        ctx.BezierTo(new Point(x4, y3), new Point(x3, y4), new Point(x2, y4), isStroked, true);
        ctx.BezierTo(new Point(x1, y4), new Point(x0, y3), new Point(x0, y2), isStroked, true);
        ctx.BezierTo(new Point(x0, y1), new Point(x1, y0), new Point(x2, y0), isStroked, true);
    }

    internal static void DrawArc(this StreamGeometryContext ctx,
        Point center, double radiusX, double radiusY,
        ArcPos pos)
    {
        var x0 = center.X - radiusX;
        var x1 = center.X - radiusX * ControlPointRatio;
        var x2 = center.X;
        var x3 = center.X + radiusX * ControlPointRatio;
        var x4 = center.X + radiusX;

        var y0 = center.Y - radiusY;
        var y1 = center.Y - radiusY * ControlPointRatio;
        var y2 = center.Y;
        var y3 = center.Y + radiusY * ControlPointRatio;
        var y4 = center.Y + radiusY;

        switch (pos)
        {
            case ArcPos.RightTop:
                ctx.BeginFigure(new Point(x2, y0), false, false);
                ctx.BezierTo(new Point(x3, y0), new Point(x4, y1), new Point(x4, y2), true, true);
                break;

            case ArcPos.LeftTop:
                ctx.BeginFigure(new Point(x2, y0), false, false);
                ctx.BezierTo(new Point(x1, y0), new Point(x0, y1), new Point(x0, y2), true, true);
                break;

            case ArcPos.RightBottom:
                ctx.BeginFigure(new Point(x2, y4), false, false);
                ctx.BezierTo(new Point(x3, y4), new Point(x4, y3), new Point(x4, y2), true, true);
                break;

            case ArcPos.LeftBottom:
                ctx.BeginFigure(new Point(x2, y4), false, false);
                ctx.BezierTo(new Point(x1, y4), new Point(x0, y3), new Point(x0, y2), true, true);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DrawTriangle(this StreamGeometryContext ctx,
        Point pos1, Point pos2, Point pos3, bool isStroked, bool isClosed)
    {
        ctx.BeginFigure(pos1, true, isClosed);
        ctx.LineTo(pos2, isStroked, false);
        ctx.LineTo(pos3, isStroked, false);
    }
}

internal enum ArcPos
{
    RightTop,
    LeftTop,
    RightBottom,
    LeftBottom
}
