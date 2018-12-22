using System.Collections.Generic;
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

        private INodeItem _srcNodeItem;
        private BiaNodePort _srcPort;

        private INodeItem _targetNodeItem;
        private BiaNodePort _targetPort;

        private bool IsDragging => _srcNodeItem != null;

        public LinkOperator(Panel renderTarget, IHasTransform transformTarget)
        {
            _renderTarget = renderTarget;
            _transformTarget = transformTarget;
        }

        internal void BeginDrag(INodeItem nodeItem, BiaNodePort port)
        {
            _srcNodeItem = nodeItem;
            _srcPort = port;

            _targetNodeItem = null;
            _targetPort = null;
        }

        internal void EndDrag()
        {
            _srcNodeItem = null;
            _srcPort = null;

            _targetNodeItem = null;
            _targetPort = null;

            _renderTarget.InvalidateVisual();
        }

        private Point _mousePos;

        public void OnLinkMoving(object sender, MouseOperator.LinkMovingEventArgs e, IEnumerable<INodeItem> nodeItems)
        {
            _mousePos = _transformTarget.MakeScenePosFromControlPos(e.MousePos);

            UpdateLinkTarget(_mousePos, nodeItems);

            _renderTarget.InvalidateVisual();
        }

        private readonly Point[] _bezierPoints = new Point[4];

        internal void Render(DrawingContext dc)
        {
            if (IsDragging == false)
                return;

            var (srcPos, srcDir) = _srcNodeItem.MakePortPos(_srcPort);

            _bezierPoints[0] = srcPos;
            _bezierPoints[1] = NodeEditorHelper.MakeBezierControlPoint(srcPos, srcDir);

            if (_targetPort == null)
            {
                _bezierPoints[2] = _mousePos;
                _bezierPoints[3] = _mousePos;
            }
            else
            {
                var targetPortPos = NodeEditorHelper.MakeNodePortPos(_targetNodeItem, _targetPort);

                _bezierPoints[2] = NodeEditorHelper.MakeBezierControlPoint(targetPortPos, _targetPort.Dir);
                _bezierPoints[3] = targetPortPos;
            }

            // 接続線
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.Black, 8));
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.WhiteSmoke, 6));

            // 接続元ポートの丸
            var portPen = Caches.GetBorderPen(Colors.Black, 2);
            dc.DrawEllipse(
                Brushes.WhiteSmoke,
                portPen,
                srcPos,
                Biaui.Internals.Constants.NodePanelPortMarkRadius_Highlight,
                Biaui.Internals.Constants.NodePanelPortMarkRadius_Highlight);

            // 接続先ポートの丸
            if (_targetPort != null)
            {
                dc.DrawEllipse(
                    Brushes.WhiteSmoke,
                    portPen,
                    _bezierPoints[3],
                    Biaui.Internals.Constants.NodePanelPortMarkRadius_Highlight,
                    Biaui.Internals.Constants.NodePanelPortMarkRadius_Highlight);
            }
        }

        protected void UpdateLinkTarget(Point mousePos, IEnumerable<INodeItem> nodeItems)
        {
            if (nodeItems == null)
            {
                _targetNodeItem = null;
                _targetPort = null;
                return;
            }

            _targetNodeItem = null;
            _targetPort = null;

            const double PortRadius = Biaui.Internals.Constants.NodePanelPortMarkRadius;

            foreach (var nodeItem in nodeItems)
            {
                var nodePos = nodeItem.Pos;
                if (mousePos.X < nodePos.X - PortRadius) continue;
                if (mousePos.Y < nodePos.Y - PortRadius) continue;

                var nodeSize = nodeItem.Size;
                if (mousePos.X > nodePos.X + nodeSize.Width + PortRadius) continue;
                if (mousePos.Y > nodePos.Y + nodeSize.Height + PortRadius) continue;

                _targetNodeItem = nodeItem;

                foreach (var port in nodeItem.Layout.Ports.Values)
                {
                    if (port == _srcPort && _targetNodeItem == _srcNodeItem)
                        continue;

                    var portPos = NodeEditorHelper.MakeNodePortPos(_targetNodeItem, port);

                    if ((portPos, mousePos).DistanceSq() > Biaui.Internals.Constants.NodePanelPortMarkRadiusSq)
                        continue;

                    _targetPort = port;

                    break;
                }

                if (_targetPort != null)
                    break;
            }
        }
    }
}