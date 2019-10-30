using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Interfaces
{
    public interface IBiaHasPos
    {
        Point Pos { get; set; }
    }

    public static class BiaHasPosExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point AlignPos(this IBiaHasPos self)
            => BiaHasPosHelper.AlignPos(self.Pos);
    }

    public static class BiaHasPosHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point AlignPos(in Point pos)
            => new Point(AlignPos(pos.X), AlignPos(pos.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AlignPos(double pos)
        {
            const double align = Internals.Constants.NodePanelAlignSize;

            return Math.Round(pos / align, MidpointRounding.AwayFromZero) * align;
        }
    }
}