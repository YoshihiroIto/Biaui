using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Biaui.Internals
{
    [DebuggerDisplay("X:{CenterX}, Y:{CenterY}, Radius:{Radius}")]
    public readonly struct ImmutableCircle : IEquatable<ImmutableCircle>
    {
        public readonly double CenterX;
        public readonly double CenterY;
        public readonly double Radius;

        public ImmutableCircle(double centerX, double centerY, double radius)
            => (CenterX, CenterY, Radius) = (centerX, centerY, radius);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static bool operator ==(in ImmutableCircle source1, in ImmutableCircle source2)
            => source1.CenterX == source2.CenterX &&
               source1.CenterY == source2.CenterY &&
               source1.Radius == source2.Radius;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(in ImmutableCircle source1, in ImmutableCircle source2)
            => !(source1 == source2);

        public bool Equals(ImmutableCircle other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is ImmutableCircle other)
                return this == other;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => HashCodeMaker.Make(CenterX, CenterY, Radius);
    }
}