using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class NodeEditorHelper
    {
        internal static Point MakeAlignPos(BiaNodePortDir dir, BiaNodePortAlign align, double width, double height)
        {
            switch (dir)
            {
                case BiaNodePortDir.Left:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(0, height / 2);
                        case BiaNodePortAlign.End:
                            return new Point(0, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Top:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(width / 2, 0);
                        case BiaNodePortAlign.End:
                            return new Point(width, 0);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Right:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(width, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(width, height / 2);
                        case BiaNodePortAlign.End:
                            return new Point(width, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Bottom:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, height);
                        case BiaNodePortAlign.Center:
                            return new Point(width / 2, height);
                        case BiaNodePortAlign.End:
                            return new Point(width, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        internal static Point MakeNodePortLocalPos(BiaNodePort port, double width, double height)
        {
            switch (port.Align)
            {
                case BiaNodePortAlign.Start:
                    var startPos = MakeAlignPos(port.Dir, BiaNodePortAlign.Start, width, height);
                    return new Point(port.Offset.X + startPos.X, port.Offset.Y + startPos.Y);

                case BiaNodePortAlign.Center:
                    var centerPos = MakeAlignPos(port.Dir, BiaNodePortAlign.Center, width, height);
                    return new Point(port.Offset.X + centerPos.X, port.Offset.Y + centerPos.Y);

                case BiaNodePortAlign.End:
                    var endPos = MakeAlignPos(port.Dir, BiaNodePortAlign.End, width, height);
                    return new Point(port.Offset.X + endPos.X, port.Offset.Y + endPos.Y);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static Point MakeNodePortPos(INodeItem nodeItem, BiaNodePort port)
        {
            var portLocalPos = MakeNodePortLocalPos(port, nodeItem.Size.Width, nodeItem.Size.Height);

            return new Point(portLocalPos.X + nodeItem.Pos.X, portLocalPos.Y + nodeItem.Pos.Y);
        }

        private const double ControlPointLength = 200;

        internal static Point MakeBezierControlPoint(Point src, BiaNodePortDir dir)
        {
            switch (dir)
            {
                case BiaNodePortDir.Left:
                    return new Point(src.X - ControlPointLength, src.Y);

                case BiaNodePortDir.Top:
                    return new Point(src.X, src.Y - ControlPointLength);

                case BiaNodePortDir.Right:
                    return new Point(src.X + ControlPointLength, src.Y);

                case BiaNodePortDir.Bottom:
                    return new Point(src.X, src.Y + ControlPointLength);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static bool HitTestBezier(Point[] bezierPoints, in ImmutableRect rect)
        {
            while (true)
            {
                if (rect.Contains(bezierPoints[0]) || rect.Contains(bezierPoints[3]))
                    return true;

                var area = new ImmutableRect(bezierPoints);

                if (rect.IntersectsWith(area) == false)
                    return false;

                var v01 = new Point((bezierPoints[0].X + bezierPoints[1].X) * 0.5,
                    (bezierPoints[0].Y + bezierPoints[1].Y) * 0.5);
                var v12 = new Point((bezierPoints[1].X + bezierPoints[2].X) * 0.5,
                    (bezierPoints[1].Y + bezierPoints[2].Y) * 0.5);
                var v23 = new Point((bezierPoints[2].X + bezierPoints[3].X) * 0.5,
                    (bezierPoints[2].Y + bezierPoints[3].Y) * 0.5);

                var v0112 = new Point((v01.X + v12.X) * 0.5, (v01.Y + v12.Y) * 0.5);
                var v1223 = new Point((v12.X + v23.X) * 0.5, (v12.Y + v23.Y) * 0.5);

                var c = new Point((v0112.X + v1223.X) * 0.5, (v0112.Y + v1223.Y) * 0.5);

                var cl = new[]
                {
                    bezierPoints[0],
                    v01,
                    v0112,
                    c
                };

                if (HitTestBezier(cl, rect))
                    return true;

                cl[0] = c;
                cl[1] = v1223;
                cl[2] = v23;
                cl[3] = bezierPoints[3];

                bezierPoints = cl;
            }
        }

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