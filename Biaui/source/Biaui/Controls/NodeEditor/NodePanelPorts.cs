using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public class NodePanelPorts : Border
    {
        static NodePanelPorts()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodePanelPorts),
                new FrameworkPropertyMetadata(typeof(NodePanelPorts)));
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
                var (startPos, centerPos, endPos) =
                    NodeEditorHelper.MakeAlignPos(BiaNodePortDir.Left, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.LeftPorts, startPos, centerPos, endPos);
            }

            if (layout.TopPorts != null)
            {
                var (startPos, centerPos, endPos) =
                    NodeEditorHelper.MakeAlignPos(BiaNodePortDir.Top, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.TopPorts, startPos, centerPos, endPos);
            }

            if (layout.RightPorts != null)
            {
                var (startPos, centerPos, endPos) =
                    NodeEditorHelper.MakeAlignPos(BiaNodePortDir.Right, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.RightPorts, startPos, centerPos, endPos);
            }

            if (layout.BottomPorts != null)
            {
                var (startPos, centerPos, endPos) =
                    NodeEditorHelper.MakeAlignPos(BiaNodePortDir.Bottom, ActualWidth, ActualHeight);

                DrawPortMarks(dc, layout.BottomPorts, startPos, centerPos, endPos);
            }
        }

        private void DrawPortMarks(
            DrawingContext dc, BiaNodePort[] ports, Point startPos, Point centerPos, Point endPos)
        {
            foreach (var port in ports)
            {
                var pos = NodeEditorHelper.MakePortMarkPoint(port.Offset, port.Align, startPos, centerPos, endPos);

                DrawPortMark(dc, pos);
            }
        }

        private void DrawPortMark(DrawingContext dc, Point pos)
        {
            var pen = Caches.GetBorderPen(Colors.Black, 2);

            dc.DrawEllipse(
                Brushes.White,
                pen,
                pos,
                8,
                8);
        }
    }
}