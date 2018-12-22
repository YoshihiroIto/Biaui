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

        public void Render(DrawingContext dc)
        {
            if (IsDragging == false)
                return;

            var (pos, dir) = _nodeItem.MakePortPos(_port);

            dc.DrawLine(Caches.GetBorderPen(Colors.Black, 8), pos, _mousePos );
            dc.DrawLine(Caches.GetBorderPen(Colors.AliceBlue, 6), pos, _mousePos );
        }
    }
}