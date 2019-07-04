using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Internals
{
    [DebuggerDisplay("X:{X}, Y:{Y}")]
    public readonly struct ImmutableVec2 : IEquatable<ImmutableVec2>
    {
        public readonly double X;
        public readonly double Y;

        public double Length => Math.Sqrt(X * X + Y * Y);
        public double LengthSq => X * X + Y * Y;

        public static ImmutableVec2 Zero = new ImmutableVec2();

        public ImmutableVec2(double x, double y)
            => (X, Y) = (x, y);

        public ImmutableVec2(Point p)
            => (X, Y) = (p.X, p.Y);

        public ImmutableVec2(Size p)
            => (X, Y) = (p.Width, p.Height);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ImmutableVec2 source1, in ImmutableVec2 source2)
            => source1.X == source2.X &&
               source1.Y == source2.Y;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ImmutableVec2 source1, in ImmutableVec2 source2)
            => !(source1 == source2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ImmutableVec2 other)
            => this == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is ImmutableVec2 other)
                return this == other;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 SetSize(in ImmutableVec2 v, double size)
        {
            var n = v.LengthSq;

            if (NumberHelper.AreCloseZero(n))
                return new ImmutableVec2(0, 0);

            var l = Math.Sqrt(n);

            return new ImmutableVec2(size * v.X / l, size * v.Y / l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 Lerp(double ratio, in ImmutableVec2 v1, ImmutableVec2 v2)
            => new ImmutableVec2(
                (v2.X - v1.X) * ratio + v1.X,
                (v2.Y - v1.Y) * ratio + v1.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 operator +(in ImmutableVec2 v1, in ImmutableVec2 v2)
            => new ImmutableVec2(v1.X + v2.X, v1.Y + v2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 operator -(in ImmutableVec2 v1, in ImmutableVec2 v2)
            => new ImmutableVec2(v1.X - v2.X, v1.Y - v2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 operator *(in ImmutableVec2 v1, double v2)
            => new ImmutableVec2(v1.X * v2, v1.Y * v2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableVec2 operator /(in ImmutableVec2 v1, double v2)
            => new ImmutableVec2(v1.X / v2, v1.Y / v2);
    }
}