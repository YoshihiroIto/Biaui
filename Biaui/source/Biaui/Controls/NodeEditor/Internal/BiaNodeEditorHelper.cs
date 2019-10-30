using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    public static class BiaNodeEditorHelper
    {
        private const double ControlPointLength = 200;

        internal static ImmutableVec2 MakeBezierControlPoint(in ImmutableVec2 src, BiaNodeSlotDir dir)
        {
            switch (dir)
            {
                case BiaNodeSlotDir.Left:
                    return new ImmutableVec2(src.X - ControlPointLength, src.Y);

                case BiaNodeSlotDir.Top:
                    return new ImmutableVec2(src.X, src.Y - ControlPointLength);

                case BiaNodeSlotDir.Right:
                    return new ImmutableVec2(src.X + ControlPointLength, src.Y);

                case BiaNodeSlotDir.Bottom:
                    return new ImmutableVec2(src.X, src.Y + ControlPointLength);

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
            Span<ImmutableVec2> cl = stackalloc ImmutableVec2[4];

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

                cl[0] = bezierPoints[0];
                cl[1] = v01;
                cl[2] = v0112;
                cl[3] = c;

                if (HitTestBezierInternal(cl, rect))
                    return true;

                bezierPoints[0] = c;
                bezierPoints[1] = v1223;
                bezierPoints[2] = v23;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableVec2 InterpolationBezier(in ImmutableVec2 p1, in ImmutableVec2 p2, in ImmutableVec2 p3, in ImmutableVec2 p4, double t)
        {
            return new ImmutableVec2(
                Bezier(p1.X, p2.X, p3.X, p4.X, t),
                Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Bezier(double x1, double x2, double x3, double x4, double t)
        {
            var mt2 = (1 - t) * (1 - t);
            var mt3 = mt2 * (1 - t);
            var pt2 = t * t;
            var pt3 = pt2 * t;

            return mt3 * x1 + 3 * mt2 * t * x2 + 3 * (1 - t) * pt2 * x3 + pt3 * x4;
        }

        // 参考： https://floris.briolas.nl/floris/2009/10/bounding-box-of-cubic-bezier/ 
        public static ImmutableRect MakeBoundingBox(in ImmutableVec2 p1, in ImmutableVec2 c1, in ImmutableVec2 c2, in ImmutableVec2 p2)
        {
            var aX = A(p1.X, c1.X, c2.X, p2.X);
            var bX = B(p1.X, c1.X, c2.X);
            var cX = C(p1.X, c1.X);

            var aY = A(p1.Y, c1.Y, c2.Y, p2.Y);
            var bY = B(p1.Y, c1.Y, c2.Y);
            var cY = C(p1.Y, c1.Y);

            var resX = Solve(aX, bX, cX).Where(t => t >= 0 && t <= 1);
            var resY = Solve(aY, bY, cY).Where(t => t >= 0 && t <= 1);

            var bBox = new List<ImmutableVec2> {p1, p2};

            foreach (var e in resX.Union(resY))
            {
                var x = Bezier(p1.X, c1.X, c2.X, p2.X, e);
                var y = Bezier(p1.Y, c1.Y, c2.Y, p2.Y, e);

                var p = new ImmutableVec2(x, y);
                bBox.Add(p);
            }

            return new ImmutableRect(bBox);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double A(double p0, double p1, double p2, double p3) => 3 * p3 - 9 * p2 + 9 * p1 - 3 * p0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double B(double p0, double p1, double p2) => 6 * p2 - 12 * p1 + 6 * p0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double C(double p0, double p1) => 3 * p1 - 3 * p0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Determinant(double a, double b, double c) => Math.Pow(b, 2) - 4d * a * c;

        // ReSharper disable InconsistentNaming
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double _SolveP(double a_, double b_, double c_) => (-b_ + Math.Sqrt(b_ * b_ - 4d * a_ * c_) * +1d) / (2d * a_);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double _SolveM(double a_, double b_, double c_) => (-b_ + Math.Sqrt(b_ * b_ - 4d * a_ * c_) * -1d) / (2d * a_);

        private static double[] Solve(double a, double b, double c)
        {
            var d = Determinant(a, b, c);

            if (d < 0)
                return Array.Empty<double>();

            if (NumberHelper.AreCloseZero(a))
                return new[] {-c / b};

            if (NumberHelper.AreCloseZero(d))
                return new[] {_SolveP(a, b, c)};

            return new[]
            {
                _SolveP(a, b, c),
                _SolveM(a, b, c)
            };
        }
        // ReSharper restore InconsistentNaming
    }
}