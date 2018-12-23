using System;
using System.Windows;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeEditorHelper
    {
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

        internal static bool HitTestBezier(Span<ImmutableVec2> bezierPoints, in ImmutableRect rect)
        {
            while (true)
            {
                if (rect.Contains(bezierPoints[0]) || rect.Contains(bezierPoints[3]))
                    return true;

                // ReSharper disable once RedundantCast
                var area = new ImmutableRect(bezierPoints, (ImmutableRect.CtorPoint4) 0);

                if (rect.IntersectsWith(area) == false)
                    return false;

                var v01 = new ImmutableVec2(
                    (bezierPoints[0].X + bezierPoints[1].X) * 0.5,
                    (bezierPoints[0].Y + bezierPoints[1].Y) * 0.5);
                var v12 = new ImmutableVec2(
                    (bezierPoints[1].X + bezierPoints[2].X) * 0.5,
                    (bezierPoints[1].Y + bezierPoints[2].Y) * 0.5);
                var v23 = new ImmutableVec2(
                    (bezierPoints[2].X + bezierPoints[3].X) * 0.5,
                    (bezierPoints[2].Y + bezierPoints[3].Y) * 0.5);

                var v0112 = new ImmutableVec2((v01.X + v12.X) * 0.5, (v01.Y + v12.Y) * 0.5);
                var v1223 = new ImmutableVec2((v12.X + v23.X) * 0.5, (v12.Y + v23.Y) * 0.5);

                var c = new ImmutableVec2((v0112.X + v1223.X) * 0.5, (v0112.Y + v1223.Y) * 0.5);

                Span<ImmutableVec2> cl = stackalloc ImmutableVec2[]
                {
                    bezierPoints[0],
                    v01,
                    v0112,
                    c
                };

                if (HitTestBezier(cl, rect))
                    return true;

                bezierPoints[0] = c;
                bezierPoints[1] = v1223;
                bezierPoints[2] = v23;
            }
        }
    }
}