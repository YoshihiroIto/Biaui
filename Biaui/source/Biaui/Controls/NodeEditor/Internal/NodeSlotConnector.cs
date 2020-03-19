using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        internal readonly ImmutableVec2_double[] BezierPoints = new ImmutableVec2_double[4];
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

            _parent.PreviewMouseUp += (_, __) => _mousePos = new ImmutableVec2_double(double.NaN, double.NaN);

            _sourceNotifier = new PropertyChangeNotifier(_parent, BiaNodeEditor.SourceNodeSlotConnectingProperty);
            _targetNotifier = new PropertyChangeNotifier(_parent, BiaNodeEditor.TargetNodeSlotConnectingProperty);

            _sourceNotifier.ValueChanged += ConnectionChangedHandler;
            _targetNotifier.ValueChanged += ConnectionChangedHandler;
        }

        private void ConnectionChangedHandler(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private ImmutableVec2_double _mousePos = new ImmutableVec2_double(double.NaN, double.NaN);

        internal void OnLinkMoving(object? sender, MouseOperator.LinkMovingEventArgs e)
        {
            var p = _parent.TransformPos(e.MousePos.X, e.MousePos.Y);
            _mousePos = Unsafe.As<Point, ImmutableVec2_double>(ref p);

            UpdateLinkTarget(_mousePos, (IEnumerable<IBiaNodeItem>?) _parent.NodesSource);

            Invalidate();
        }

        internal void UpdateBezierPoints()
        {
            if (IsNodeSlotDragging == false)
                return;

            var srcPos = _parent.SourceNodeSlotConnecting.MakeSlotPos();

            BezierPoints[0] = srcPos;

            if (_parent.TargetNodeSlotConnecting.IsNull)
            {
                var handleLength = srcPos.Distance(_mousePos) * 0.5d;
                
                BezierPoints[1] = BiaNodeEditorHelper.MakeBezierControlPoint(srcPos, _parent.SourceNodeSlotConnecting.Slot.Dir, handleLength);
                BezierPoints[2] = _mousePos;
                BezierPoints[3] = _mousePos;
            }
            else
            {
                var targetSlotPos = _parent.TargetNodeSlotConnecting.MakeSlotPos();
                
                var handleLength = srcPos.Distance(targetSlotPos) * 0.5d;

                BezierPoints[1] = BiaNodeEditorHelper.MakeBezierControlPoint(srcPos, _parent.SourceNodeSlotConnecting.Slot.Dir, handleLength);
                BezierPoints[2] = BiaNodeEditorHelper.MakeBezierControlPoint(targetSlotPos, _parent.TargetNodeSlotConnecting.Slot.Dir, handleLength);
                BezierPoints[3] = targetSlotPos;
            }
        }

        internal void Render(in LayoutRounder rounder, DrawingContext dc, in ImmutableRect_double rect, double scale)
        {
            if (IsNodeSlotDragging == false)
                return;

            if (double.IsNaN(_mousePos.X))
                return;

            var invScale = 1d / Scale.ScaleX;

            // 接続線
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(ByteColor.Black, 5d * invScale));
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(ByteColor.WhiteSmoke, 3d * invScale));

            // 接続元ポートの丸
            {
                var isSourceDesktopSpace = _parent.SourceNodeSlotConnecting.Item.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace);
                var sourceRadius = Biaui.Internals.Constants.SlotMarkRadius_Highlight2(isSourceDesktopSpace) * scale;

                if (isSourceDesktopSpace)
                    sourceRadius *= invScale;

                var srcRect = new ImmutableRect_double(
                    BezierPoints[0].X - sourceRadius, BezierPoints[0].Y - sourceRadius,
                    sourceRadius * 2d, sourceRadius * 2d);

                if (rect.IntersectsWith(srcRect))
                {
                    var drawRadius = Biaui.Internals.Constants.SlotMarkRadius_Highlight2(isSourceDesktopSpace);
                    
                    if (isSourceDesktopSpace)
                         drawRadius *= invScale;

                    var penWidth = isSourceDesktopSpace ? 2d * invScale : 2d;
                    var slotPen = Caches.GetPen(ByteColor.Black, rounder.RoundLayoutValue(penWidth));
                    
                    dc.DrawCircle(
                        Caches.GetSolidColorBrush(_parent.SourceNodeSlotConnecting.Slot.Color),
                        slotPen,
                        Unsafe.As<ImmutableVec2_double, Point>(ref BezierPoints[0]),
                        drawRadius);
                }
            }

            // 接続先ポートの丸
            if (_parent.TargetNodeSlotConnecting.IsNotNull)
            {
                var isTargetDesktopSpace = _parent.TargetNodeSlotConnecting.Item.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace);
                var targetRadius = Biaui.Internals.Constants.SlotMarkRadius_Highlight2(isTargetDesktopSpace) * scale;

                if (isTargetDesktopSpace)
                    targetRadius *= invScale;

                var targetRect = new ImmutableRect_double(
                    BezierPoints[3].X - targetRadius, BezierPoints[3].Y - targetRadius,
                    targetRadius * 2d, targetRadius * 2d);

                if (rect.IntersectsWith(targetRect))
                {
                    var drawRadius = Biaui.Internals.Constants.SlotMarkRadius_Highlight2(isTargetDesktopSpace);
                    if (isTargetDesktopSpace)
                         drawRadius *= invScale;
                    
                    var penWidth = isTargetDesktopSpace ? 2d * invScale : 2d;
                    var slotPen = Caches.GetPen(ByteColor.Black, rounder.RoundLayoutValue(penWidth));
                    
                    dc.DrawCircle(
                        Caches.GetSolidColorBrush(_parent.TargetNodeSlotConnecting.Slot.Color),
                        slotPen,
                        Unsafe.As<ImmutableVec2_double, Point>(ref BezierPoints[3]),
                        drawRadius);
                }
            }
        }

        private void UpdateLinkTarget(in ImmutableVec2_double mousePos, IEnumerable<IBiaNodeItem>? nodeItems)
        {
            _parent.TargetNodeSlotConnecting = default;

            if (nodeItems == null)
                return;

            var invScale = 1d / Scale.ScaleX;

            foreach (var nodeItem in nodeItems)
            {
                var slotRadius = Biaui.Internals.Constants.SlotMarkRadius(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));

                if (nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
                    slotRadius *= invScale;

                var nodePos = nodeItem.Pos;
                if (mousePos.X < nodePos.X - slotRadius) continue;
                if (mousePos.Y < nodePos.Y - slotRadius) continue;

                var nodeSize = nodeItem.Size;
                if (mousePos.X > nodePos.X + nodeSize.Width + slotRadius) continue;
                if (mousePos.Y > nodePos.Y + nodeSize.Height + slotRadius) continue;

                foreach (var slot in nodeItem.EnabledSlots())
                {
                    if (slot == _parent.SourceNodeSlotConnecting.Slot &&
                        nodeItem == _parent.SourceNodeSlotConnecting.Item)
                        continue;

                    var slotPos = nodeItem.MakeSlotPosDefault(slot);

                    if (slot.HitCheck(invScale, nodeItem, slotPos, mousePos))
                    {
                        _parent.TargetNodeSlotConnecting = new BiaNodeItemSlotPair(nodeItem, slot);
                        break;
                    }
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

        internal ImmutableRect_double Transform(in ImmutableRect_double rect) => _parent.TransformRect(rect);

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

            var rounder = new LayoutRounder(this);

            var scale = _parent.Scale.ScaleX;
            var rect = _parent.Transform(new ImmutableRect_double(0, 0, ActualWidth, ActualHeight));
            rect = new ImmutableRect_double(rect.X + Pos.X / scale, rect.Y + Pos.Y / scale, rect.Width, rect.Height);

            Span<ImmutableVec2_double> bezierPoints = stackalloc ImmutableVec2_double[4];
            var hitTestWork = bezierPoints;

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
                        _parent.Render(rounder, dc, rect, scale);
                    }
                    dc.Pop();
                    dc.Pop();
                }
                dc.Pop();
            }
        }
    }
}