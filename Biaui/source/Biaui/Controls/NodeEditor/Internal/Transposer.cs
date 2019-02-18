using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class Transposer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double X(this Point self, bool isTrue)
            => isTrue
                ? self.X
                : self.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Y(this Point self, bool isTrue)
            => isTrue
                ? self.Y
                : self.X;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double X(this ImmutableVec2 self, bool isTrue)
            => isTrue
                ? self.X
                : self.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Y(this ImmutableVec2 self, bool isTrue)
            => isTrue
                ? self.Y
                : self.X;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Width(this Size self, bool isTrue)
            => isTrue
                ? self.Width
                : self.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Height(this Size self, bool isTrue)
            => isTrue
                ? self.Height
                : self.Width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static BiaNodeSlotDir NodeSlotDir(BiaNodeSlotDir target, bool isTrue)
        {
            if (isTrue)
                return target;

            switch (target)
            {
                case BiaNodeSlotDir.Left:
                    return BiaNodeSlotDir.Top;

                case BiaNodeSlotDir.Top:
                    return BiaNodeSlotDir.Left;

                case BiaNodeSlotDir.Right:
                    return BiaNodeSlotDir.Bottom;

                case BiaNodeSlotDir.Bottom:
                    return BiaNodeSlotDir.Right;

                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableVec2 CreateImmutableVec2(double x, double y, bool isTrue)
            => isTrue
                ? new ImmutableVec2(x, y)
                : new ImmutableVec2(y, x);
    }
}