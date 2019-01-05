using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Controls.NodeEditor;
using Biaui.Internals;

namespace Biaui.Controls.Internals
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
        internal static BiaNodePortDir NodePortDir(BiaNodePortDir target, bool isTrue)
        {
            if (isTrue)
                return target;

            switch (target)
            {
                case BiaNodePortDir.Left:
                    return BiaNodePortDir.Top;

                case BiaNodePortDir.Top:
                    return BiaNodePortDir.Left;

                case BiaNodePortDir.Right:
                    return BiaNodePortDir.Bottom;

                case BiaNodePortDir.Bottom:
                    return BiaNodePortDir.Right;

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