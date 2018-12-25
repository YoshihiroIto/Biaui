using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class LinkOperator
    {
        private readonly Panel _renderTarget;
        private readonly IHasTransform _transformTarget;

        public IBiaNodeItem SourceItem { get; private set; }

        public BiaNodePort SourcePort { get; private set; }

        public IBiaNodeItem TargetItem { get; private set; }

        public BiaNodePort TargetPort { get; private set; }

        public bool IsDragging => SourceItem != null;

        public LinkOperator(Panel renderTarget, IHasTransform transformTarget)
        {
            _renderTarget = renderTarget;
            _transformTarget = transformTarget;
        }

        internal void BeginDrag(IBiaNodeItem nodeItem, BiaNodePort port)
        {
            SourceItem = nodeItem ?? throw new ArgumentNullException(nameof(nodeItem));
            SourcePort = port ?? throw new ArgumentNullException(nameof(port));

            TargetItem = null;
            TargetPort = null;
        }

        internal void EndDrag()
        {
            SourceItem = null;
            SourcePort = null;

            TargetItem = null;
            TargetPort = null;

            _renderTarget.InvalidateVisual();
        }

        private Point _mousePos;

        public bool OnLinkMoving(object sender, MouseOperator.LinkMovingEventArgs e,
            IEnumerable<IBiaNodeItem> nodeItems)
        {
            _mousePos = _transformTarget.TransformPos(e.MousePos.X, e.MousePos.Y);

            var changed = UpdateLinkTarget(_mousePos, nodeItems);

            _renderTarget.InvalidateVisual();

            return changed;
        }

        private readonly Point[] _bezierPoints = new Point[4];

        internal void Render(DrawingContext dc)
        {
            if (IsDragging == false)
                return;

            var srcPos = SourceItem.MakePortPos(SourcePort);

            _bezierPoints[0] = srcPos;
            _bezierPoints[1] = BiaNodeEditorHelper.MakeBezierControlPoint(srcPos, SourcePort.Dir);

            if (TargetPort == null)
            {
                _bezierPoints[2] = _mousePos;
                _bezierPoints[3] = _mousePos;
            }
            else
            {
                var targetPortPos = TargetItem.MakePortPos(TargetPort);

                _bezierPoints[2] = BiaNodeEditorHelper.MakeBezierControlPoint(targetPortPos, TargetPort.Dir);
                _bezierPoints[3] = targetPortPos;
            }

            // 接続線
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.Black, 8));
            dc.DrawBezier(_bezierPoints, Caches.GetCapPen(Colors.WhiteSmoke, 6));

            // 接続元ポートの丸
            var portPen = Caches.GetBorderPen(Colors.Black, 2);
            dc.DrawEllipse(
                Caches.GetSolidColorBrush(SourcePort.Color),
                portPen,
                srcPos,
                Biaui.Internals.Constants.PortMarkRadius_Highlight,
                Biaui.Internals.Constants.PortMarkRadius_Highlight);

            // 接続先ポートの丸
            if (TargetPort != null)
            {
                dc.DrawEllipse(
                    Caches.GetSolidColorBrush(TargetPort.Color),
                    portPen,
                    _bezierPoints[3],
                    Biaui.Internals.Constants.PortMarkRadius_Highlight,
                    Biaui.Internals.Constants.PortMarkRadius_Highlight);
            }
        }

        protected bool UpdateLinkTarget(Point mousePos, IEnumerable<IBiaNodeItem> nodeItems)
        {
            var oldTargetItem = TargetItem;
            var oldTargetPort = TargetPort;

            TargetItem = null;
            TargetPort = null;

            if (nodeItems != null)
            {
                TargetItem = null;
                TargetPort = null;

                const double PortRadius = Biaui.Internals.Constants.PortMarkRadius;

                foreach (var nodeItem in nodeItems)
                {
                    var nodePos = nodeItem.Pos;
                    if (mousePos.X < nodePos.X - PortRadius) continue;
                    if (mousePos.Y < nodePos.Y - PortRadius) continue;

                    var nodeSize = nodeItem.Size;
                    if (mousePos.X > nodePos.X + nodeSize.Width + PortRadius) continue;
                    if (mousePos.Y > nodePos.Y + nodeSize.Height + PortRadius) continue;

                    TargetItem = nodeItem;

                    foreach (var port in nodeItem.Layout.Ports.Values)
                    {
                        if (port == SourcePort && TargetItem == SourceItem)
                            continue;

                        var portPos = TargetItem.MakePortPos(port);

                        if ((portPos, mousePos).DistanceSq() > Biaui.Internals.Constants.PortMarkRadiusSq)
                            continue;

                        TargetPort = port;

                        break;
                    }

                    if (TargetPort != null)
                        break;
                }
            }

            return oldTargetItem != TargetItem ||
                   oldTargetPort != TargetPort;
        }
    }
}