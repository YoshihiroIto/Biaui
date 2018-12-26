using System;
using System.Diagnostics;
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

        public LinkConnector(LinkOperator linkOperator, IHasTransform transform)
        {
            _linkOperator = linkOperator;
            _transform = transform;

            {
                var i = 0;

                for (var y = 0; y != RowCount; ++y)
                for (var x = 0; x != ColumnCount; ++x, ++i)
                {
                    Children.Add(
                        new LinkConnectorCell(this)
                        {
                            Width = 100,
                            Height = 100,
                            Index = i
                        }
                    );
                }
            }

            SizeChanged += (_, e) =>
            {
                var cellWidth = e.NewSize.Width / ColumnCount;
                var cellHeight = e.NewSize.Height / RowCount;

                var i = 0;

                for (var y = 0; y != RowCount; ++y)
                for (var x = 0; x != ColumnCount; ++x, ++i)
                {
                    var c = (LinkConnectorCell) Children[i];
                    c.Width = cellWidth;
                    c.Height = cellHeight;

                    SetLeft(c, cellWidth * x);
                    SetTop(c, cellHeight * y);

                    c.Pos = new Point(cellWidth * x, cellHeight * y); 
                }

                InvalidateMeasure();
            };
        }

        internal void Invalidate()
        {
            //InvalidateMeasure();

            foreach (var child in Children)
                ((FrameworkElement)child).InvalidateVisual();

            Debug.WriteLine($"{c++}");
        }

        private int c;

        internal void Render(DrawingContext dc)
        {
            _linkOperator.Render(dc);
        }

        internal Point[] BezierPoints
        {
            get
            {
                _linkOperator.UpdateBezierPoints();
                return _linkOperator.BezierPoints;
            }
        }

        public bool IsDragging => _linkOperator.IsDragging;

        public ImmutableRect Transform(in ImmutableRect rect)
        {
            return _transform.TransformRect(rect);
        }
    }

    internal class LinkConnectorCell : Control
    {
        private readonly LinkConnector _parent;

        public int Index { get; set; }

        public Point Pos { get; set; }

        public LinkConnectorCell(LinkConnector parent)
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
                dc.PushTransform(new TranslateTransform(-Pos.X, -Pos.Y));
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

            //    dc.DrawRectangle(null, Caches.GetPen(Colors.BlueViolet, 1), this.RoundLayoutActualRectangle(false));
            }

            //TextRenderer.Default.Draw(isHit.ToString(), 0, 0, Brushes.WhiteSmoke, dc, ActualWidth, TextAlignment.Left);
        }
    }
}