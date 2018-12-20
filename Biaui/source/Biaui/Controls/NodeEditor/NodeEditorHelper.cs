using System;
using System.Windows;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    internal static class NodeEditorHelper
    {
        internal static Point MakePortMarkPoint(
            Point basePos, BiaNodePortAlign align, Point startPos, Point centerPos, Point endPos)
        {
            switch (align)
            {
                case BiaNodePortAlign.Start:
                    return new Point(basePos.X + startPos.X, basePos.Y + startPos.Y);

                case BiaNodePortAlign.Center:
                    return new Point(basePos.X + centerPos.X, basePos.Y + centerPos.Y);

                case BiaNodePortAlign.End:
                    return new Point(basePos.X + endPos.X, basePos.Y + endPos.Y);

                default:
                    throw new ArgumentOutOfRangeException(nameof(align), align, null);
            }
        }

        internal static (Point StartPos, Point CenterPos, Point EndPos)
            MakeAlignPos(BiaNodePortDir dir, double width, double height)
        {
            switch (dir)
            {
                case BiaNodePortDir.Left:

                    return (
                        new Point(0, 0),
                        new Point(0, height / 2),
                        new Point(0, height));

                case BiaNodePortDir.Top:
                    return (
                        new Point(0, 0),
                        new Point(width / 2, 0),
                        new Point(width, 0));

                case BiaNodePortDir.Right:

                    return (
                        new Point(width, 0),
                        new Point(width, height / 2),
                        new Point(width, height));

                case BiaNodePortDir.Bottom:
                    return (
                        new Point(0, height),
                        new Point(width / 2, height),
                        new Point(width, height));

                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}