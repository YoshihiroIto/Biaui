using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeEditorHelper
    {
        private const double ControlPointLength = 200;

        internal static Point MakeBezierControlPoint(Point src, BiaNodeSlotDir dir)
        {
            switch (dir)
            {
                case BiaNodeSlotDir.Left:
                    return new Point(src.X - ControlPointLength, src.Y);

                case BiaNodeSlotDir.Top:
                    return new Point(src.X, src.Y - ControlPointLength);

                case BiaNodeSlotDir.Right:
                    return new Point(src.X + ControlPointLength, src.Y);

                case BiaNodeSlotDir.Bottom:
                    return new Point(src.X, src.Y + ControlPointLength);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool HitTestBezier(Span<ImmutableVec2> bezierPoints, in ImmutableRect rect)
        {
            if (NumberHelper.AreCloseZero(rect.Width) ||
                NumberHelper.AreCloseZero(rect.Height))
                return false;

            return HitTestBezierInternal(bezierPoints, rect);
        }

        private static bool HitTestBezierInternal(Span<ImmutableVec2> bezierPoints, in ImmutableRect rect)
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

                if (NumberHelper.Abs(v0112.X - v1223.X) < 0.00001 &&
                    NumberHelper.Abs(v0112.Y - v1223.Y) < 0.00001)
                    return rect.IntersectsWith(v0112);

                var c = new ImmutableVec2((v0112.X + v1223.X) * 0.5, (v0112.Y + v1223.Y) * 0.5);

                Span<ImmutableVec2> cl = new[]
                {
                    bezierPoints[0],
                    v01,
                    v0112,
                    c
                };

                if (HitTestBezierInternal(cl, rect))
                    return true;

                bezierPoints[0] = c;
                bezierPoints[1] = v1223;
                bezierPoints[2] = v23;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableVec2 InterpolationBezier(Point p1, Point p2, Point p3, Point p4, double t)
        {
            return new ImmutableVec2(
                Bezier(p1.X, p2.X, p3.X, p4.X, t),
                Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Bezier(double x1, double x2, double x3, double x4, double t)
        {
            var mt2 = (1 - t) * (1 - t);
            var mt3 = mt2 * (1 - t);
            var pt2 = t * t;
            var pt3 = pt2 * t;

            return mt3 * x1 + 3 * mt2 * t * x2 + 3 * (1 - t) * pt2 * x3 + pt3 * x4;
        }
    }
}