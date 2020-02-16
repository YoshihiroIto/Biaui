using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Jewelry.Collections;

namespace Biaui.Internals
{
    internal static class DrawingContextExtensions
    {
        internal static void DrawBezier(this DrawingContext dc, ImmutableVec2_double[ /*4*/] bezierPoints, Pen pen)
        {
            var hashCode = MakeHashCode(bezierPoints);

            if (_bezierCache.TryGetValue(hashCode, out var curve) == false)
            {
                curve = new StreamGeometry();

                using (var ctx = curve.Open())
                {
                    ctx.BeginFigure(
                        Unsafe.As<ImmutableVec2_double, Point>(ref bezierPoints[0]),
                        false,
                        false
                    );

                    ctx.BezierTo(
                        Unsafe.As<ImmutableVec2_double, Point>(ref bezierPoints[1]),
                        Unsafe.As<ImmutableVec2_double, Point>(ref bezierPoints[2]),
                        Unsafe.As<ImmutableVec2_double, Point>(ref bezierPoints[3]),
                        true,
                        false);
                }

                curve.Freeze();

                _bezierCache.Add(hashCode, curve);
            }

            dc.DrawGeometry(null, pen, curve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MakeHashCode(Span<ImmutableVec2_double> src)
        {
            unchecked
            {
                var srcPrim = MemoryMarshal.Cast<ImmutableVec2_double, double>(src);

                var p0X = Unsafe.As<double, long>(ref srcPrim[0]);
                var p0Y = Unsafe.As<double, long>(ref srcPrim[1]);
                var p1X = Unsafe.As<double, long>(ref srcPrim[2]);
                var p1Y = Unsafe.As<double, long>(ref srcPrim[3]);
                var p2X = Unsafe.As<double, long>(ref srcPrim[4]);
                var p2Y = Unsafe.As<double, long>(ref srcPrim[5]);
                var p3X = Unsafe.As<double, long>(ref srcPrim[6]);
                var p3Y = Unsafe.As<double, long>(ref srcPrim[7]);

                var hashCode = p0X;

                hashCode = (hashCode * 397) ^ p0Y;
                hashCode = (hashCode * 397) ^ p1X;
                hashCode = (hashCode * 397) ^ p1Y;
                hashCode = (hashCode * 397) ^ p2X;
                hashCode = (hashCode * 397) ^ p2Y;
                hashCode = (hashCode * 397) ^ p3X;
                hashCode = (hashCode * 397) ^ p3Y;

                return hashCode;
            }
        }

        internal static void DrawCircle(this DrawingContext dc, Brush brush, Pen pen, Point pos, double radius)
        {
            var geom = new StreamGeometry
            {
                FillRule = FillRule.Nonzero
            };

            using (var ctx = geom.Open())
            {
                ctx.DrawEllipse(
                    pos,
                    radius,
                    radius,
                    true,
                    true);
            }

            dc.DrawGeometry(brush, pen, geom);
        }

        private static readonly LruCache<long, StreamGeometry> _bezierCache = new LruCache<long, StreamGeometry>(10000);
    }
}