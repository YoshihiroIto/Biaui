using System;

namespace Biaui.Internals
{
    public readonly struct ImmutablePoint : IEquatable<ImmutablePoint>
    {
        public readonly double X;
        public readonly double Y;

        public ImmutablePoint(double x, double y)
            => (X, Y) = (x, y);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static bool operator ==(in ImmutablePoint source1, in ImmutablePoint source2)
            => source1.X == source2.X &&
               source1.Y == source2.Y;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(in ImmutablePoint source1, in ImmutablePoint source2)
            => !(source1 == source2);

        public bool Equals(ImmutablePoint other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is ImmutablePoint other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => X.GetHashCode() ^
               Y.GetHashCode();
    }
}