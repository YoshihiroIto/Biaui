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
    internal class LinkConnector : Canvas
    {
        static LinkConnector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkConnector),
                new FrameworkPropertyMetadata(typeof(LinkConnector)));
        }

        internal IBiaNodeItem SourceItem { get; private set; }

        internal BiaNodePort SourcePort { get; private set; }

        internal IBiaNodeItem TargetItem { get; private set; }

        internal BiaNodePort TargetPort { get; private set; }

        internal TranslateTransform Translate => _transform.Translate;

        internal ScaleTransform Scale => _transform.Scale;

        internal bool IsDragging => SourceItem != null;

        internal readonly Point[] BezierPoints = new Point[4];

        private const int ColumnCount = 8;
        private const int RowCount = 8;
        private readonly IHasTransform _transform;

        internal LinkConnector(IHasTransform transform)
        {
            _transform = transform;

            for (var i = 0; i != RowCount * ColumnCount; ++i)
                Children.Add(new LinkConnectorCell(this));

            SizeChanged += (_, e) =>
            {
                UpdateChildren(e.NewSize.Width, e.NewSize.Height);
                InvalidateMeasure();
            };
        }

        internal void BeginDrag(IBiaNodeItem nodeItem, BiaNodePort port)
        {
            SourceItem = nodeItem ?? throw new ArgumentNullException(nameof(nodeItem));
            SourcePort = port ?? throw new ArgumentNullException(nameof(port));

            TargetItem = null;
            TargetPort = null;

            Invalidate();
        }

        internal void EndDrag()
        {
            SourceItem = null;
            SourcePort = null;

            TargetItem = null;
            TargetPort = null;

            Invalidate();
        }

        private Point _mousePos;

        internal bool OnLinkMoving(object sender, MouseOperator.LinkMovingEventArgs e,
            IEnumerable<IBiaNodeItem> nodeItems)
        {
            _mousePos = _transform.TransformPos(e.MousePos.X, e.MousePos.Y);

            var changed = UpdateLinkTarget(_mousePos, nodeItems);

            Invalidate();

            return changed;
        }

        internal void UpdateBezierPoints()
        {
            if (IsDragging == false)
                return;

            var srcPos = SourceItem.MakePortPos(SourcePort);

            BezierPoints[0] = srcPos;
            BezierPoints[1] = BiaNodeEditorHelper.MakeBezierControlPoint(srcPos, SourcePort.Dir);

            if (TargetPort == null)
            {
                BezierPoints[2] = _mousePos;
                BezierPoints[3] = _mousePos;
            }
            else
            {
                var targetPortPos = TargetItem.MakePortPos(TargetPort);

                BezierPoints[2] = BiaNodeEditorHelper.MakeBezierControlPoint(targetPortPos, TargetPort.Dir);
                BezierPoints[3] = targetPortPos;
            }
        }

        internal void Render(DrawingContext dc, in ImmutableRect rect, double scale)
        {
            if (IsDragging == false)
                return;

            var radius = Biaui.Internals.Constants.PortMarkRadius_Highlight * scale;

            // 接続線
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(Colors.Black, 5));
            dc.DrawBezier(BezierPoints, Caches.GetCapPen(Colors.WhiteSmoke, 3));

            var portPen = Caches.GetPen(Colors.Black, 2);

            // 接続元ポートの丸
            var srcRect = new ImmutableRect(
                BezierPoints[0].X - radius, BezierPoints[0].Y - radius, radius * 2, radius * 2);

            if (rect.IntersectsWith(srcRect))
            {
                dc.DrawEllipse(
                    Caches.GetSolidColorBrush(SourcePort.Color),
                    portPen,
                    BezierPoints[0],
                    Biaui.Internals.Constants.PortMarkRadius_Highlight,
                    Biaui.Internals.Constants.PortMarkRadius_Highlight);
            }

            // 接続先ポートの丸
            if (TargetPort != null)
            {
                var targetRect = new ImmutableRect(
                    BezierPoints[3].X - radius, BezierPoints[3].Y - radius, radius * 2, radius * 2);

                if (rect.IntersectsWith(targetRect))
                {
                    dc.DrawEllipse(
                        Caches.GetSolidColorBrush(TargetPort.Color),
                        portPen,
                        BezierPoints[3],
                        Biaui.Internals.Constants.PortMarkRadius_Highlight,
                        Biaui.Internals.Constants.PortMarkRadius_Highlight);
                }
            }
        }

        private bool UpdateLinkTarget(Point mousePos, IEnumerable<IBiaNodeItem> nodeItems)
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

        internal void Invalidate()
        {
            UpdateBezierPoints();

            UpdateChildren(ActualWidth, ActualHeight);

            foreach (var child in Children)
                ((FrameworkElement) child).InvalidateVisual();
        }

        internal ImmutableRect Transform(in ImmutableRect rect) => _transform.TransformRect(rect);

        private void UpdateChildren(double width, double height)
        {
            var cellWidth = width / ColumnCount;
            var cellHeight = height / RowCount;
            var margin = Biaui.Internals.Constants.PortMarkRadius_Highlight * _transform.Scale.ScaleX;

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
        private readonly LinkConnector _parent;

        internal Point Pos { get; set; }

        internal new double Margin { get; set; }

        internal LinkConnectorCell(LinkConnector parent)
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

            if (_parent.IsDragging == false)
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

            try
            {
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

                    dc.DrawRectangle(null, Caches.GetPen(Colors.BlueViolet, 1), this.RoundLayoutActualRectangle(false));
                }

                //TextRenderer.Default.Draw(isHit.ToString(), 0, 0, Brushes.WhiteSmoke, dc, ActualWidth, TextAlignment.Left);
            }
            catch (StackOverflowException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}