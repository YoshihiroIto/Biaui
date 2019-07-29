using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class NodeSlotConnector : Canvas
    {
        static NodeSlotConnector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeSlotConnector),
                new FrameworkPropertyMetadata(typeof(NodeSlotConnector)));
        }

        internal TranslateTransform Translate => _parent.TranslateTransform;

        internal ScaleTransform Scale => _parent.ScaleTransform;

        public bool IsNodeSlotDragging => _parent.SourceNodeSlotConnecting.IsNotNull;

        internal readonly Point[] BezierPoints = new Point[4];
        private readonly BiaNodeEditor _parent;

        private const int ColumnCount = 8;
        private const int RowCount = 8;

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly PropertyChangeNotifier _sourceNotifier;
        private readonly PropertyChangeNotifier _targetNotifier;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        internal NodeSlotConnector(BiaNodeEditor parent, MouseOperator mouseOperator)
        {
            _parent = parent;

            for (var i = 0; i != RowCount * ColumnCount; ++i)
                Children.Add(new LinkConnectorCell(this));

            mouseOperator.LinkMoving += OnLinkMoving;
            SizeChanged += (_, e) => UpdateChildren(e.NewSize.Width, e.NewSize.Height);

            _parent.PreviewMouseUp += (_, __) => _mousePos = new Point(double.NaN, double.NaN);

            _sourceNotifier = new PropertyChangeNotifier(_parent, BiaNodeEditor.SourceNodeSlotConnectingProperty);
            _targetNotifier = new PropertyChangeNotifier(_parent, BiaNodeEditor.TargetNodeSlotConnectingProperty);

            _sourceNotifier.ValueChanged += ConnectionChangedHandler;
            _targetNotifier.ValueChanged += ConnectionChangedHandler;
        }

        private void ConnectionChangedHandler(object sender, EventArgs e)
        {
            Invalidate();
        }

        private Point _mousePos = new Point(double.NaN, double.NaN);

        internal void OnLinkMoving(object sender, MouseOperator.LinkMovingEventArgs e)
        {
            _mousePos = _parent.TransformPos(e.MousePos.X, e.MousePos.Y);

            UpdateLinkTarget(_mousePos, (IEnumerable<IBiaNodeItem>)_parent.NodesSource);

            Invalidate();
        }

        internal void UpdateBezierPoints()
        {
            if (IsNodeSlotDragging == false)
                return;

            var srcPos = _parent.SourceNodeSlotConnecting.MakeSlotPos();

            BezierPoints[0] = srcPos;
            BezierPoints[1] = BiaNodeEditorHelper.MakeBezierControlPoint(srcPos, _parent.SourceNodeSlotConnecting.Slot.Dir);

            if (_parent.TargetNodeSlotConnecting.IsNull)
            {
                BezierPoints[2] = _mousePos;
                BezierPoints[3] = _mousePos;
            }
            else
            {
                var targetSlotPos = _parent.TargetNodeSlotConnecting.MakeSlotPos();

                BezierPoints[2] = BiaNodeEditorHelper.MakeBezierControlPoint(targetSlotPos, _parent.TargetNodeSlotConnecting.Slot.Dir);
                BezierPoints[3] = targetSlotPos;
            }
        }

        internal void Render(DrawingContext dc, in ImmutableRect rect, double scale)
        {
            if (IsNodeSlotDragging == false)
                return;

            if (double.IsNaN(_mousePos.X))
                return;

            var radius = Biaui.Internals.Constants.SlotMarkRadius_Highlight2 * scale;

            // 接続線
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(Colors.Black, 5));
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(Colors.WhiteSmoke, 3));

            var slotPen = Caches.GetPen(Colors.Black, 2);

            // 接続元ポートの丸
            var srcRect = new ImmutableRect(
                BezierPoints[0].X - radius, BezierPoints[0].Y - radius, radius * 2, radius * 2);

            if (rect.IntersectsWith(srcRect))
            {
                dc.DrawCircle(
                    Caches.GetSolidColorBrush(_parent.SourceNodeSlotConnecting.Slot.Color),
                    slotPen,
                    BezierPoints[0],
                    Biaui.Internals.Constants.SlotMarkRadius_Highlight2);
            }

            // 接続先ポートの丸
            if (_parent.TargetNodeSlotConnecting.IsNotNull)
            {
                var targetRect = new ImmutableRect(
                    BezierPoints[3].X - radius, BezierPoints[3].Y - radius, radius * 2, radius * 2);

                if (rect.IntersectsWith(targetRect))
                {
                    dc.DrawCircle(
                        Caches.GetSolidColorBrush(_parent.TargetNodeSlotConnecting.Slot.Color),
                        slotPen,
                        BezierPoints[3],
                        Biaui.Internals.Constants.SlotMarkRadius_Highlight2);
                }
            }
        }

        private void UpdateLinkTarget(Point mousePos, IEnumerable<IBiaNodeItem> nodeItems)
        {
            _parent.TargetNodeSlotConnecting = default;

            if (nodeItems == null)
                return;

            const double SlotRadius = Biaui.Internals.Constants.SlotMarkRadius;

            foreach (var nodeItem in nodeItems)
            {
                var nodePos = nodeItem.Pos;
                if (mousePos.X < nodePos.X - SlotRadius) continue;
                if (mousePos.Y < nodePos.Y - SlotRadius) continue;

                var nodeSize = nodeItem.Size;
                if (mousePos.X > nodePos.X + nodeSize.Width + SlotRadius) continue;
                if (mousePos.Y > nodePos.Y + nodeSize.Height + SlotRadius) continue;

                foreach (var slot in nodeItem.EnabledSlots())
                {
                    if (slot == _parent.SourceNodeSlotConnecting.Slot &&
                        nodeItem == _parent.SourceNodeSlotConnecting.Item)
                        continue;

                    var slotPos = nodeItem.MakeSlotPos(slot);

                    if (slot.HitCheck(slotPos, mousePos) == false)
                        continue;

                    _parent.TargetNodeSlotConnecting = new BiaNodeItemSlotPair(nodeItem, slot);

                    break;
                }

                if (_parent.TargetNodeSlotConnecting != null)
                    break;
            }
        }

        internal void Invalidate()
        {
            UpdateBezierPoints();

            for (var i = 0; i != RowCount * ColumnCount; ++i)
            {
                var child = (LinkConnectorCell) Children[i];
                child.InvalidateVisual();
            }

            _parent.IsNodeSlotDragging = IsNodeSlotDragging;
            _parent.InvokeLinkChanged();
        }

        internal ImmutableRect Transform(in ImmutableRect rect) => _parent.TransformRect(rect);

        private void UpdateChildren(double width, double height)
        {
            var cellWidth = width / ColumnCount;
            var cellHeight = height / RowCount;
            var margin = Biaui.Internals.Constants.SlotMarkRadius_Max * _parent.ScaleTransform.ScaleX;

            var i = 0;

            for (var y = 0; y != RowCount; ++y)
            for (var x = 0; x != ColumnCount; ++x, ++i)
            {
                var child = (LinkConnectorCell) Children[i];
                child.Width = cellWidth + margin * 2;
                child.Height = cellHeight + margin * 2;

                SetLeft(child, cellWidth * x - margin);
                SetTop(child, cellHeight * y - margin);

                child.Pos = new Point(cellWidth * x, cellHeight * y);
                child.Margin = margin;
            }
        }
    }

    internal class LinkConnectorCell : FrameworkElement
    {
        private readonly NodeSlotConnector _parent;

        internal Point Pos { get; set; }

        internal new double Margin { get; set; }

        internal LinkConnectorCell(NodeSlotConnector parent)
        {
            IsHitTestVisible = false;
            _parent = parent;

            ClipToBounds = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            if (_parent.IsNodeSlotDragging == false)
                return;

            var scale = _parent.Scale.ScaleX;
            var rect = _parent.Transform(new ImmutableRect(0, 0, ActualWidth, ActualHeight));
            rect = new ImmutableRect(rect.X + Pos.X / scale, rect.Y + Pos.Y / scale, rect.Width, rect.Height);

            Span<Point> bezierPoints = stackalloc Point[4];
            var hitTestWork = MemoryMarshal.Cast<Point, ImmutableVec2>(bezierPoints);

            bezierPoints[0] = _parent.BezierPoints[0];
            bezierPoints[1] = _parent.BezierPoints[1];
            bezierPoints[2] = _parent.BezierPoints[2];
            bezierPoints[3] = _parent.BezierPoints[3];

            var isHit = BiaNodeEditorHelper.HitTestBezier(hitTestWork, rect);

            if (isHit)
            {
                dc.PushTransform(new TranslateTransform(-Pos.X + Margin, -Pos.Y + Margin));
                {
                    dc.PushTransform(_parent.Translate);
                    dc.PushTransform(_parent.Scale);
                    {
                        _parent.Render(dc, rect, scale);
                    }
                    dc.Pop();
                    dc.Pop();
                }
                dc.Pop();

                //dc.DrawRectangle(null, Caches.GetPen(Colors.BlueViolet, 1), this.RoundLayoutActualRectangle(false));
            }

            //TextRenderer.Default.Draw(isHit.ToString(), 0, 0, Brushes.WhiteSmoke, dc, ActualWidth, TextAlignment.Left);
        }
    }
}