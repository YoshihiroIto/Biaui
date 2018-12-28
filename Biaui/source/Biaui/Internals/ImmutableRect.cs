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

        public ImmutableRect(Point[] poss)
        {
            var minX = poss[0].X;
            var minY = poss[0].Y;
            var maxX = poss[0].X;
            var maxY = poss[0].Y;

            for (var i = 1; i < poss.Length; ++i)
            {
                minX = (minX, poss[i].X).Min();
                maxX = (maxX, poss[i].X).Max();

                minY = (minY, poss[i].Y).Min();
                maxY = (maxY, poss[i].Y).Max();
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
        public ImmutableRect(Point[] poss, CtorPoint4 _)
        {
            var minX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Min();
            var maxX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Max();
            var minY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Min();
            var maxY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Max();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        // ReSharper disable once UnusedParameter.Local
        public ImmutableRect(ReadOnlySpan<Point> poss, CtorPoint4 _)
        {
            var minX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Min();
            var maxX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Max();
            var minY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Min();
            var maxY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Max();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        // ReSharper disable once UnusedParameter.Local
        public ImmutableRect(ReadOnlySpan<ImmutableVec2> poss, CtorPoint4 _)
        {
            var minX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Min();
            var maxX = (poss[0].X, poss[1].X, poss[2].X, poss[3].X).Max();
            var minY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Min();
            var maxY = (poss[0].Y, poss[1].Y, poss[2].Y, poss[3].Y).Max();

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public Rect ToRect()
            => new Rect(X, Y, Width, Height);

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