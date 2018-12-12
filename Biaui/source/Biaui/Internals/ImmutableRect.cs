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

        public ImmutableRect(double x, double y, double width, double height) =>
            (X, Y, Width, Height) = (x, y, width, height);

        public ImmutableRect(Point p, Size s) =>
            (X, Y, Width, Height) = (p.X, p.Y, s.Width, s.Height);

        public static implicit operator ImmutableRect(Rect source) =>
            new ImmutableRect(source.X, source.Y, source.Width, source.Height);

        public static implicit operator Rect(in ImmutableRect source) =>
            new Rect(source.X, source.Y, source.Width, source.Height);

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
        public bool IntersectsWith(double targetLeft, double targetTop, double targetRight, double targetBottom)
        {
            var right = X + Width;
            var bottom = Y + Height;

            return targetLeft <= right && targetRight >= X && targetTop <= bottom && targetBottom >= Y;
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static bool operator ==(in ImmutableRect source1, in ImmutableRect source2) =>
            source1.X == source2.X && source1.Y == source2.Y && source1.Width == source2.Width &&
            source1.Height == source2.Height;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(in ImmutableRect source1, in ImmutableRect source2) => !(source1 == source2);

        public bool Equals(ImmutableRect other) => this == other;

        public override bool Equals(object obj)
        {
            if (obj is ImmutableRect other)
            {
                return this == other;
            }

            return false;
        }

        public override int GetHashCode() =>
            X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
    }
}