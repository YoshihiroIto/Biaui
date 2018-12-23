using System;
using System.Diagnostics;
using System.Windows;

namespace Biaui.Internals
{
    [DebuggerDisplay("X:{X}, Y:{Y}")]
    public readonly struct ImmutableVec2 : IEquatable<ImmutableVec2>
    {
        public readonly double X;
        public readonly double Y;

        public ImmutableVec2(double x, double y)
            => (X, Y) = (x, y);

        public ImmutableVec2(Point p)
            => (X, Y) = (p.X, p.Y);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static bool operator ==(in ImmutableVec2 source1, in ImmutableVec2 source2)
            => source1.X == source2.X &&
               source1.Y == source2.Y;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(in ImmutableVec2 source1, in ImmutableVec2 source2)
            => !(source1 == source2);

        public bool Equals(ImmutableVec2 other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is ImmutableVec2 other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => X.GetHashCode() ^
               Y.GetHashCode();

        public Point ToPoint()
            => new Point(X, Y);
    }
}