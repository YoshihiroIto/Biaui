using System;
using System.Windows;

namespace Biaui.Interfaces
{
    public interface IBiaHasPos
    {
        Point Pos { get; set; }
    }

    public static class BiaHasPosExtensions
    {
        public static Point AlignedPos(this IBiaHasPos self)
            => BiaHasPosHelper.AlignPos(self.Pos);
    }

    public static class BiaHasPosHelper
    {
        public static Point AlignPos(Point pos)
            => new Point(AlignPos(pos.X), AlignPos(pos.Y));

        public static double AlignPos(double pos)
        {
            const double align = Internals.Constants.NodePanelAlignSize;

            return Math.Round(pos / align) * align;
        }
    }
}