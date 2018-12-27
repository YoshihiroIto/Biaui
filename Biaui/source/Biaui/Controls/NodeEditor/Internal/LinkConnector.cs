using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
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

        private const int ColumnCount = 8;
        private const int RowCount = 8;

        private readonly LinkOperator _linkOperator;
        private readonly IHasTransform _transform;

        internal TranslateTransform Translate => _transform.Translate;

        internal ScaleTransform Scale => _transform.Scale;

        internal LinkConnector(LinkOperator linkOperator, IHasTransform transform)
        {
            _linkOperator = linkOperator;
            _transform = transform;

            for (var i = 0; i != RowCount * ColumnCount; ++i)
                Children.Add(new LinkConnectorCell(this));

            SizeChanged += (_, e) =>
            {
                UpdateChildren(e.NewSize.Width, e.NewSize.Height);
                InvalidateMeasure();
            };
        }

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

        internal void Invalidate()
        {
            if (IsDragging)
                _linkOperator.UpdateBezierPoints();

            UpdateChildren(ActualWidth, ActualHeight);

            foreach (var child in Children)
                ((FrameworkElement) child).InvalidateVisual();
        }

        internal void Render(DrawingContext dc)
        {
            _linkOperator.Render(dc);
        }

        internal Point[] BezierPoints => _linkOperator.BezierPoints;

        internal bool IsDragging => _linkOperator.IsDragging;

        internal ImmutableRect Transform(in ImmutableRect rect)
        {
            return _transform.TransformRect(rect);
        }
    }

    internal class LinkConnectorCell : Control
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

            var s = _parent.Scale.ScaleX;
            var rect = _parent.Transform(new ImmutableRect(0, 0, ActualWidth, ActualHeight));
            rect = new ImmutableRect(rect.X + Pos.X / s, rect.Y + Pos.Y / s, rect.Width, rect.Height);

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
                        _parent.Render(dc);
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