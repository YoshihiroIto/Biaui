using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class LinkOperator
    {
        private readonly Panel _renderTarget;
        private readonly IHasTransform _transformTarget;

        private INodeItem _nodeItem;
        private BiaNodePort _port;

        private bool IsDragging => _nodeItem != null;

        public LinkOperator(Panel renderTarget, IHasTransform transformTarget)
        {
            _renderTarget = renderTarget;
            _transformTarget = transformTarget;
        }

        internal void BeginDrag(INodeItem nodeItem, BiaNodePort port)
        {
            _nodeItem = nodeItem;
            _port = port;
        }

        internal void EndDrag()
        {
            _nodeItem = null;
            _port = null;

            _renderTarget.InvalidateVisual();
        }

        private Point _mousePos;

        public void OnLinkMoving(object sender, MouseOperator.LinkMovingEventArgs e)
        {
            _mousePos = _transformTarget.MakeScenePosFromControlPos(e.MousePos);

            _renderTarget.InvalidateVisual();
        }

        private readonly Point[] _bezierPoints = new Point[4];

        public void Render(DrawingContext dc)
        {
            if (IsDragging == false)
                return;

            var (pos, dir) = _nodeItem.MakePortPos(_port);

            _bezierPoints[0] = pos;
            _bezierPoints[1] = NodeEditorHelper.MakeControlPoint(pos, dir);
            _bezierPoints[2] = _mousePos;
            _bezierPoints[3] = _mousePos;

            // 接続線
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.Black, 8));
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.WhiteSmoke, 6));

            // ポートの丸
            var portPen = Caches.GetBorderPen(Colors.Black, 2);
            var radius = Biaui.Internals.Constants.NodePanelPortMarkRadius * 1.25;
            dc.DrawEllipse(
                Brushes.WhiteSmoke,
                portPen,
                pos,
                radius, 
                radius);
        }
    }
}