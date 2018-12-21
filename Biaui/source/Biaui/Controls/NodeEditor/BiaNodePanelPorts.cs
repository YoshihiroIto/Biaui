using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanelPorts : Border
    {
        static BiaNodePanelPorts()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodePanelPorts),
                new FrameworkPropertyMetadata(typeof(BiaNodePanelPorts)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var pen = Caches.GetBorderPen(Colors.Black, 2);

            var nodeItem = (INodeItem) DataContext;

            foreach (var port in nodeItem.Layout.Ports.Values)
            {
                Point pos;

                switch (port.Align)
                {
                    case BiaNodePortAlign.Start:
                        var startPos = NodeEditorHelper.MakeAlignPos(
                            port.Dir, BiaNodePortAlign.Start, ActualWidth, ActualHeight);
                        pos = new Point(port.Offset.X + startPos.X, port.Offset.Y + startPos.Y);
                        break;

                    case BiaNodePortAlign.Center:
                        var centerPos = NodeEditorHelper.MakeAlignPos(
                            port.Dir, BiaNodePortAlign.Center, ActualWidth, ActualHeight);
                        pos = new Point(port.Offset.X + centerPos.X, port.Offset.Y + centerPos.Y);
                        break;

                    case BiaNodePortAlign.End:
                        var endPos = NodeEditorHelper.MakeAlignPos(
                            port.Dir, BiaNodePortAlign.End, ActualWidth, ActualHeight);
                        pos = new Point(port.Offset.X + endPos.X, port.Offset.Y + endPos.Y);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                dc.DrawEllipse(
                    Brushes.White,
                    pen,
                    pos,
                    8,
                    8);
            }
        }

        private void DrawPortMarks(DrawingContext dc, BiaNodePort[] ports, Point startPos, Point centerPos,
            Point endPos)
        {
            var pen = Caches.GetBorderPen(Colors.Black, 2);

            foreach (var port in ports)
            {
                Point pos;

                switch (port.Align)
                {
                    case BiaNodePortAlign.Start:
                        pos = new Point(port.Offset.X + startPos.X, port.Offset.Y + startPos.Y);
                        break;

                    case BiaNodePortAlign.Center:
                        pos = new Point(port.Offset.X + centerPos.X, port.Offset.Y + centerPos.Y);
                        break;

                    case BiaNodePortAlign.End:
                        pos = new Point(port.Offset.X + endPos.X, port.Offset.Y + endPos.Y);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                dc.DrawEllipse(
                    Brushes.White,
                    pen,
                    pos,
                    8,
                    8);
            }
        }
    }
}