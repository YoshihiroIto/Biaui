using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.NodeEditor.Internal;
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

        private Point _mousePoint;

        internal void UpdateMousePos(Point point)
        {
            _mousePoint = point;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var pen = Caches.GetBorderPen(Colors.Black, 2);

            var nodeItem = (INodeItem) DataContext;

            var isMouseOverNode = nodeItem.IsMouseOver;

            foreach (var port in nodeItem.Layout.Ports.Values)
            {
                var portPos = NodeEditorHelper.MakeNodePortLocalPos(port, ActualWidth, ActualHeight);

                var r = Biaui.Internals.Constants.NodePanelPortMarkRadius;

                // パネルがマウスオーバー時は、ポート自体のマウス位置見て半径を作る
                if (isMouseOverNode)
                {
                    if ((portPos, _mousePoint).DistanceSq() <=
                        Biaui.Internals.Constants.NodePanelPortMarkRadiusSq)
                        r = Biaui.Internals.Constants.NodePanelPortMarkRadius_Highlight;
                }

                dc.DrawEllipse(
                    Brushes.WhiteSmoke,
                    pen,
                    portPos,
                    r, 
                    r);
            }
        }
    }
}