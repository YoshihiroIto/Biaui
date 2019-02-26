using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Internals
{
    // http://proprogrammer.hatenadiary.jp/entry/2018/08/18/172739

    public readonly struct ImmutableRect: IEquatable<ImmutableRect>
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Width;
        public readonly double Height;

        public bool HasArea => Width > 0 && Height > 0;

        public ImmutableRect(double x, double y, double width, double height)
            => (X, Y, Width, Height) = (x, y, width, height);

        public ImmutableRect(Point pos, Size size)
            => (X, Y, Width, Height) = (pos.X, pos.Y, size.Width, size.Height);

        public ImmutableRect(Point pos0, Point pos1)
        {
            var (minX, maxX) = (pos0.X, pos1.X).MinMax();
            var (minY, maxY) = (pos0.Y, pos1.Y).MinMax();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public ImmutableRect(in ImmutableVec2 pos0, in ImmutableVec2 pos1)
        {
            var (minX, maxX) = (pos0.X, pos1.X).MinMax();
            var (minY, maxY) = (pos0.Y, pos1.Y).MinMax();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public ImmutableRect(Span<Point> points)
        {
            if (points.Length == 0)
            {
                (X, Y, Width, Height) = (0, 0, 0, 0);
                return;
            }

            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            for (var i = 1; i < points.Length; ++i)
            {
                minX = (minX, points[i].X).Min();
                maxX = (maxX, points[i].X).Max();

                minY = (minY, points[i].Y).Min();
                maxY = (maxY, points[i].Y).Max();
            }

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public ImmutableRect(ReadOnlySpan<Point> points)
        {
            if (points.Length == 0)
            {
                (X, Y, Width, Height) = (0, 0, 0, 0);
                return;
            }

            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            for (var i = 1; i < points.Length; ++i)
            {
                minX = (minX, points[i].X).Min();
                maxX = (maxX, points[i].X).Max();

                minY = (minY, points[i].Y).Min();
                maxY = (maxY, points[i].Y).Max();
            }

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public ImmutableRect(Span<ImmutableVec2> points)
        {
            if (points.Length == 0)
            {
                (X, Y, Width, Height) = (0, 0, 0, 0);
                return;
            }

            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            for (var i = 1; i < points.Length; ++i)
            {
                minX = (minX, points[i].X).Min();
                maxX = (maxX, points[i].X).Max();

                minY = (minY, points[i].Y).Min();
                maxY = (maxY, points[i].Y).Max();
            }

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public ImmutableRect(ReadOnlySpan<ImmutableVec2> points)
        {
            if (points.Length == 0)
            {
                (X, Y, Width, Height) = (0, 0, 0, 0);
                return;
            }

            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            for (var i = 1; i < points.Length; ++i)
            {
                minX = (minX, points[i].X).Min();
                maxX = (maxX, points[i].X).Max();

                minY = (minY, points[i].Y).Min();
                maxY = (maxY, points[i].Y).Max();
            }

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }


        public enum CtorPoint4 { };

        // ReSharper disable once UnusedParameter.Local
        public ImmutableRect(ReadOnlySpan<Point> points, CtorPoint4 _)
        {
            var minX = (points[0].X, points[1].X, points[2].X, points[3].X).Min();
            var maxX = (points[0].X, points[1].X, points[2].X, points[3].X).Max();
            var minY = (points[0].Y, points[1].Y, points[2].Y, points[3].Y).Min();
            var maxY = (points[0].Y, points[1].Y, points[2].Y, points[3].Y).Max();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        // ReSharper disable once UnusedParameter.Local
        public ImmutableRect(ReadOnlySpan<ImmutableVec2> points, CtorPoint4 _)
        {
            var minX = (points[0].X, points[1].X, points[2].X, points[3].X).Min();
            var maxX = (points[0].X, points[1].X, points[2].X, points[3].X).Max();
            var minY = (points[0].Y, points[1].Y, points[2].Y, points[3].Y).Min();
            var maxY = (points[0].Y, points[1].Y, points[2].Y, points[3].Y).Max();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsWith(in ImmutableRect target)
        {
            var right = X + Width;
            var bottom = Y + Height;
            var targetRight = target.X + target.Width;
            var targetBottom = target.Y + target.Height;

            return target.X <= right && targetRight >= X && target.Y <= bottom && targetBottom >= Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsWith(in ImmutableVec2 target)
        {
            var right = X + Width;
            var bottom = Y + Height;

            return target.X >= X &&
                   target.X < right &&
                   target.Y >= bottom &&
                   target.Y < bottom;
        }

        public bool IntersectsWith(in ImmutableCircle target)
        {
            var right = X + Width;
            var bottom = Y + Height;

            if (target.CenterX > X &&
                target.CenterX < right &&
                target.CenterY > Y - target.Radius &&
                target.CenterY < bottom + target.Radius)
                return true;

            if (target.CenterX > X - target.Radius&&
                target.CenterX < right + target.Radius &&
                target.CenterY > Y &&
                target.CenterY < bottom)
                return true;

            var rr = target.Radius * target.Radius;

            var xx1 = (X - target.CenterX) * (X - target.CenterX);
            var yy1 = (Y - target.CenterY) * (Y - target.CenterY);

            var xx2 = (right - target.CenterX) * (right - target.CenterX);
            var yy2 = (bottom - target.CenterY) * (bottom - target.CenterY);

            if (xx1 + yy1 < rr)
                return true;

            if (xx2 + yy1 < rr)
                return true;

            if (xx1 + yy2 < rr)
                return true;

            if (xx2 + yy2 < rr)
                return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point p)
            =>
                p.X >= X &&
                p.X < X + Width &&
                p.Y >= Y &&
                p.Y < Y + Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in ImmutableVec2 p)
            =>
                p.X >= X &&
                p.X < X + Width &&
                p.Y >= Y &&
                p.Y < Y + Height;

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static bool operator ==(in ImmutableRect source1, in ImmutableRect source2)
            => source1.X == source2.X &&
               source1.Y == source2.Y &&
               source1.Width == source2.Width &&
               source1.Height == source2.Height;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(in ImmutableRect source1, in ImmutableRect source2)
            => !(source1 == source2);

        public bool Equals(ImmutableRect other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is ImmutableRect other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => X.GetHashCode() ^
               Y.GetHashCode() ^
               Width.GetHashCode() ^
               Height.GetHashCode();
    }
}