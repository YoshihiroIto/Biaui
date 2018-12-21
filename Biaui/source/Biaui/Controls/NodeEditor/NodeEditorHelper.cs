using System;
using System.Windows;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    internal static class NodeEditorHelper
    {
        internal static Point MakeAlignPos(BiaNodePortDir dir, BiaNodePortAlign align, double width, double height)
        {
            switch (dir)
            {
                case BiaNodePortDir.Left:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(0, height / 2);
                        case BiaNodePortAlign.End:
                            return new Point(0, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Top:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(width / 2, 0);
                        case BiaNodePortAlign.End:
                            return new Point(width, 0);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Right:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(width, 0);
                        case BiaNodePortAlign.Center:
                            return new Point(width, height / 2);
                        case BiaNodePortAlign.End:
                            return new Point(width, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                case BiaNodePortDir.Bottom:

                    switch (align)
                    {
                        case BiaNodePortAlign.Start:
                            return new Point(0, height);
                        case BiaNodePortAlign.Center:
                            return new Point(width / 2, height);
                        case BiaNodePortAlign.End:
                            return new Point(width, height);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(align), align, null);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}