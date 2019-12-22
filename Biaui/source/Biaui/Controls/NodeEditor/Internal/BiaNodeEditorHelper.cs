using System;
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

            Span<ImmutableVec2> bBox = stackalloc ImmutableVec2[2 + 2 + 2];
            bBox[0] = p1;
            bBox[1] = p2;
            var bBoxCount = 2;

            Span<double> res = stackalloc double[2 + 2];

            var resCount = Solve(res, aX, bX, cX);
            resCount += Solve(res.Slice(resCount, res.Length - resCount), aY, bY, cY);

            for (var i = 0; i != resCount; ++i)
            {
                var x = Bezier(p1.X, c1.X, c2.X, p2.X, res[i]);
                var y = Bezier(p1.Y, c1.Y, c2.Y, p2.Y, res[i]);

                bBox[bBoxCount++] = new ImmutableVec2(x, y);
            }

            return new ImmutableRect(bBox.Slice(0, bBoxCount));
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
        private static double SolveP(double a, double b, double c) => (-b + Math.Sqrt(b * b - 4d * a * c) * +1d) / (2d * a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double SolveM(double a, double b, double c) => (-b + Math.Sqrt(b * b - 4d * a * c) * -1d) / (2d * a);

        private static int Solve(Span<double> result, double a, double b, double c)
        {
            var d = Determinant(a, b, c);

            if (d < 0)
                return 0;

            if (NumberHelper.AreCloseZero(a))
            {
                result[0] = -c / b;
                return 1;
            }

            if (NumberHelper.AreCloseZero(d))
            {
                result[0] = SolveP(a, b, c);
                return 1;
            }

            result[0] = SolveP(a, b, c);
            result[1] = SolveM(a, b, c);
            return 2;
        }

        // ReSharper restore InconsistentNaming
    }
}