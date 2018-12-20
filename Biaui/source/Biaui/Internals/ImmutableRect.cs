using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Internals
{
    // http://proprogrammer.hatenadiary.jp/entry/2018/08/18/172739

    public readonly struct ImmutableRect
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
            var (minX, maxX) = NumberHelper.MinMax(pos0.X, pos1.X);
            var (minY, maxY) = NumberHelper.MinMax(pos0.Y, pos1.Y);

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
                minX = NumberHelper.Min(minX, poss[i].X);
                maxX = NumberHelper.Max(maxX, poss[i].X);

                minY = NumberHelper.Min(minY, poss[i].Y);
                maxY = NumberHelper.Max(maxY, poss[i].Y);
            }

            (X, Y, Width, Height) = (
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );
        }

        public static implicit operator ImmutableRect(Rect source)
            => new ImmutableRect(source.X, source.Y, source.Width, source.Height);

        public static implicit operator Rect(in ImmutableRect source)
            => new Rect(source.X, source.Y, source.Width, source.Height);

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
        public bool Contains(Point p)
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