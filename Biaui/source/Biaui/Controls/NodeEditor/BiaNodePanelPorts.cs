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

            var nodeItem = (INodeItem) DataContext;

            var layout = nodeItem.Layout;

            if (layout.LeftPorts != null)
            {
                var startPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Left, BiaNodePortAlign.Start, ActualWidth, ActualHeight);
                var centerPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Left, BiaNodePortAlign.Center, ActualWidth, ActualHeight);
                var endPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Left, BiaNodePortAlign.End, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.LeftPorts, startPos, centerPos, endPos);
            }

            if (layout.TopPorts != null)
            {
                var startPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Top, BiaNodePortAlign.Start, ActualWidth, ActualHeight);
                var centerPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Top, BiaNodePortAlign.Center, ActualWidth, ActualHeight);
                var endPos = NodeEditorHelper.MakeAlignPos( BiaNodePortDir.Top, BiaNodePortAlign.End, ActualWidth,
                    ActualHeight);

                DrawPortMarks(dc, layout.TopPorts, startPos, centerPos, endPos);
            }

            if (layout.RightPorts != null)
            {
                var startPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Right, BiaNodePortAlign.Start, ActualWidth, ActualHeight);
                var centerPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Right, BiaNodePortAlign.Center, ActualWidth, ActualHeight);
                var endPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Right, BiaNodePortAlign.End, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.RightPorts, startPos, centerPos, endPos);
            }

            if (layout.BottomPorts != null)
            {
                var startPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Bottom, BiaNodePortAlign.Start, ActualWidth, ActualHeight);
                var centerPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Bottom, BiaNodePortAlign.Center, ActualWidth, ActualHeight);
                var endPos = NodeEditorHelper.MakeAlignPos(
                    BiaNodePortDir.Bottom, BiaNodePortAlign.End, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.BottomPorts, startPos, centerPos, endPos);
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