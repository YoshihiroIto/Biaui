using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaUniformGrid : Panel
    {
        #region Spacing

        public double Spacing
        {
            get => _Spacing;
            set
            {
                if (NumberHelper.AreClose(value, _Spacing) == false)
                    SetValue(SpacingProperty, Boxes.Double(value));
            }
        }

        private double _Spacing;

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                nameof(Spacing),
                typeof(double),
                typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaUniformGrid) s;
                        self._Spacing = (double) e.NewValue;
                    }));

        #endregion

        #region Columns

        public int Columns
        {
            get => _Columns;
            set
            {
                if (value != _Columns)
                    SetValue(ColumnsProperty, Boxes.Int(value));
            }
        }

        private int _Columns;

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(
                    Boxes.Int0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    (s, e) =>
                    {
                        var self = (BiaUniformGrid) s;
                        self._Columns = (int) e.NewValue;
                    }));

        #endregion

        #region Rows

        public int Rows
        {
            get => _Rows;
            set
            {
                if (value != _Rows)
                    SetValue(RowsProperty, Boxes.Int(value));
            }
        }

        private int _Rows;

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(int),
                typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(
                    Boxes.Int0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    (s, e) =>
                    {
                        var self = (BiaUniformGrid) s;
                        self._Rows = (int) e.NewValue;
                    }));

        #endregion

        #region IsFillLastRow

        public bool IsFillLastRow
        {
            get => _IsFillLastRow;
            set
            {
                if (value != _IsFillLastRow)
                    SetValue(IsFillLastRowProperty, Boxes.Bool(value));
            }
        }

        private bool _IsFillLastRow = true;

        public static readonly DependencyProperty IsFillLastRowProperty =
            DependencyProperty.Register(
                nameof(IsFillLastRow),
                typeof(bool),
                typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(
                    Boxes.BoolTrue,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    (s, e) =>
                    {
                        var self = (BiaUniformGrid) s;
                        self._IsFillLastRow = (bool) e.NewValue;
                    }));

        #endregion

        static BiaUniformGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(typeof(BiaUniformGrid)));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            var childConstraint = new Size(
                Math.Ceiling(constraint.Width / _columns),
                Math.Ceiling(constraint.Height / _rows));
            var maxChildDesiredWidth = 0.0;
            var maxChildDesiredHeight = 0.0;

            for (var i = 0; i != InternalChildren.Count; ++i)
            {
                var child = InternalChildren[i];

                child.Measure(childConstraint);

                var childDesiredSize = child.DesiredSize;

                maxChildDesiredWidth = Math.Max(maxChildDesiredWidth, childDesiredSize.Width);
                maxChildDesiredHeight = Math.Max(maxChildDesiredHeight, childDesiredSize.Height);
            }

            return new Size(maxChildDesiredWidth * _columns, maxChildDesiredHeight * _rows);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var xIndex = 0;
            var yIndex = 0;

            var (baseChildWidth, childHeight) =
                this.RoundLayoutValue(arrangeSize.Width / _columns, arrangeSize.Height / _rows);

            var dpiSpacing = Spacing * this.PixelsPerDip();
            var childBounds = new Rect();

            var fillColumns = InternalChildren.Count % _columns;
            var isLastFill = IsFillLastRow && (fillColumns == 0);

            for (var i = 0; i != InternalChildren.Count; ++i)
            {
                var child = InternalChildren[i];

                double childWidth;
                int columns;

                if (yIndex != _rows - 1 || isLastFill)
                {
                    childWidth = baseChildWidth;
                    columns = _columns;
                }
                else
                {
                    childWidth = Math.Floor(arrangeSize.Width / fillColumns);
                    columns = fillColumns;
                }

                childBounds.X = xIndex * childWidth;
                childBounds.Y = yIndex * childHeight;
                childBounds.Width = xIndex == columns - 1
                    ? childBounds.Width = arrangeSize.Width - childWidth * (columns - 1)
                    : Math.Max(0, childWidth - dpiSpacing);
                childBounds.Height = yIndex == _rows - 1
                    ? childBounds.Height = arrangeSize.Height - childHeight * (_rows - 1)
                    : Math.Max(0, childHeight - dpiSpacing);

                child.Arrange(childBounds);

                if (child.Visibility != Visibility.Collapsed)
                {
                    ++xIndex;

                    if (xIndex == _columns)
                    {
                        xIndex = 0;
                        ++yIndex;
                    }
                }
            }

            return arrangeSize;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;

            if (_rows == 0 || _columns == 0)
            {
                var nonCollapsedCount = 0;

                for (var i = 0; i != InternalChildren.Count; ++i)
                {
                    var child = InternalChildren[i];

                    if (child.Visibility != Visibility.Collapsed)
                        nonCollapsedCount++;
                }

                if (nonCollapsedCount == 0)
                    nonCollapsedCount = 1;

                if (_rows == 0)
                {
                    if (_columns > 0)
                        _rows = (nonCollapsedCount + (_columns - 1)) / _columns;

                    else
                    {
                        _columns = nonCollapsedCount;
                        _rows = 1;
                    }
                }
                else if (_columns == 0)
                    _columns = (nonCollapsedCount + (_rows - 1)) / _rows;
            }
        }

        private int _rows;
        private int _columns;

        #region 角丸用処理

        #region CornerRadius

        public CornerRadius CornerRadius
        {
            get => _CornerRadius;
            set
            {
                if (value != _CornerRadius)
                    SetValue(CornerRadiusProperty, value);
            }
        }

        private CornerRadius _CornerRadius;

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(BiaUniformGrid),
                new FrameworkPropertyMetadata(
                    default(CornerRadius),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaUniformGrid) s;
                        self._CornerRadius = (CornerRadius) e.NewValue;
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        protected virtual void OnApplyChildClip()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (CornerRadius.BottomLeft == 0.0 &&
                CornerRadius.BottomRight == 0.0 &&
                CornerRadius.TopLeft == 0.0 &&
                CornerRadius.TopRight == 0.0)
            {
                var parentClip = (Parent as UIElement)?.Clip;
                if (parentClip != null)
                    Clip = parentClip;
                return;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var rect = new Rect(RenderSize);
            var key = (rect, CornerRadius);

            if (_clipRectCache.TryGetValue(key, out var clipRect) == false)
            {
                var isSame =
                    NumberHelper.AreClose(CornerRadius.TopLeft, CornerRadius.TopRight) &&
                    NumberHelper.AreClose(CornerRadius.TopRight, CornerRadius.BottomRight) &&
                    NumberHelper.AreClose(CornerRadius.BottomRight, CornerRadius.BottomLeft) &&
                    NumberHelper.AreClose(CornerRadius.BottomLeft, CornerRadius.TopLeft);

                clipRect = isSame
                    ? MakeRoundRectangleGeometrySameCorner(new Rect(RenderSize), CornerRadius)
                    : MakeRoundRectangleGeometry(new Rect(RenderSize), CornerRadius);

                _clipRectCache.Add(key, clipRect);
            }

            Clip = clipRect;
        }

        private static readonly Dictionary<(Rect Rect, CornerRadius CornerRadius), Geometry> _clipRectCache =
            new Dictionary<(Rect Rect, CornerRadius CornerRadius), Geometry>();

        private static Geometry MakeRoundRectangleGeometrySameCorner(Rect baseRect, CornerRadius cornerRadius)
        {
            var radius = (0.0, cornerRadius.TopLeft).Max();

            var clipRect = new RectangleGeometry
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = baseRect
            };

            clipRect.Freeze();

            return clipRect;
        }

        // https://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/
        private static Geometry MakeRoundRectangleGeometry(Rect baseRect, CornerRadius cornerRadius)
        {
            if (cornerRadius.TopLeft < double.Epsilon)
                cornerRadius.TopLeft = 0.0;

            if (cornerRadius.TopRight < double.Epsilon)
                cornerRadius.TopRight = 0.0;

            if (cornerRadius.BottomLeft < double.Epsilon)
                cornerRadius.BottomLeft = 0.0;

            if (cornerRadius.BottomRight < double.Epsilon)
                cornerRadius.BottomRight = 0.0;

            var topLeftRect = new Rect(
                baseRect.Location.X,
                baseRect.Location.Y,
                (0.0, cornerRadius.TopLeft).Max(),
                (0.0, cornerRadius.TopLeft).Max());

            var topRightRect = new Rect(
                baseRect.Location.X + baseRect.Width - cornerRadius.TopRight,
                baseRect.Location.Y,
                (0.0, cornerRadius.TopRight).Max(),
                (0.0, cornerRadius.TopRight).Max());

            var bottomRightRect = new Rect(
                baseRect.Location.X + baseRect.Width - cornerRadius.BottomRight,
                baseRect.Location.Y + baseRect.Height - cornerRadius.BottomRight,
                (0.0, cornerRadius.BottomRight).Max(),
                (0.0, cornerRadius.BottomRight).Max());

            var bottomLeftRect = new Rect(
                baseRect.Location.X,
                baseRect.Location.Y + baseRect.Height - cornerRadius.BottomLeft,
                (0.0, cornerRadius.BottomLeft).Max(),
                (0.0, cornerRadius.BottomLeft).Max());

            if (topLeftRect.Right > topRightRect.Left)
            {
                var newWidth = topLeftRect.Width / (topLeftRect.Width + topRightRect.Width) * baseRect.Width;

                topLeftRect = new Rect(
                    topLeftRect.Location.X,
                    topLeftRect.Location.Y,
                    newWidth,
                    topLeftRect.Height);

                topRightRect = new Rect(
                    baseRect.Left + newWidth,
                    topRightRect.Location.Y,
                    (0.0, baseRect.Width - newWidth).Max(),
                    topRightRect.Height);
            }

            if (topRightRect.Bottom > bottomRightRect.Top)
            {
                var newHeight = topRightRect.Height / (topRightRect.Height + bottomRightRect.Height) * baseRect.Height;

                topRightRect = new Rect(
                    topRightRect.Location.X,
                    topRightRect.Location.Y,
                    topRightRect.Width,
                    newHeight);

                bottomRightRect = new Rect(
                    bottomRightRect.Location.X,
                    baseRect.Top + newHeight,
                    bottomRightRect.Width,
                    (0.0, baseRect.Height - newHeight).Max());
            }

            if (bottomRightRect.Left < bottomLeftRect.Right)
            {
                var newWidth = bottomLeftRect.Width / (bottomLeftRect.Width + bottomRightRect.Width) * baseRect.Width;

                bottomLeftRect = new Rect(
                    bottomLeftRect.Location.X,
                    bottomLeftRect.Location.Y,
                    newWidth,
                    bottomLeftRect.Height);

                bottomRightRect = new Rect(
                    baseRect.Left + newWidth,
                    bottomRightRect.Location.Y,
                    (0.0, baseRect.Width - newWidth).Max(),
                    bottomRightRect.Height);
            }

            if (bottomLeftRect.Top < topLeftRect.Bottom)
            {
                var newHeight = topLeftRect.Height / (topLeftRect.Height + bottomLeftRect.Height) * baseRect.Height;

                topLeftRect = new Rect(
                    topLeftRect.Location.X,
                    topLeftRect.Location.Y,
                    topLeftRect.Width,
                    newHeight);

                bottomLeftRect = new Rect(
                    bottomLeftRect.Location.X,
                    baseRect.Top + newHeight,
                    bottomLeftRect.Width,
                    (0.0, baseRect.Height - newHeight).Max());
            }

            var clipRect = new StreamGeometry();

            using (var context = clipRect.Open())
            {
                context.BeginFigure(topLeftRect.BottomLeft, true, true);

                context.ArcTo(topLeftRect.TopRight, topLeftRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(topRightRect.TopLeft, true, true);
                context.ArcTo(topRightRect.BottomRight, topRightRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(bottomRightRect.TopRight, true, true);
                context.ArcTo(bottomRightRect.BottomLeft, bottomRightRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(bottomLeftRect.BottomRight, true, true);
                context.ArcTo(bottomLeftRect.TopLeft, bottomLeftRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);
            }

            clipRect.Freeze();

            return clipRect;
        }

        #endregion
    }
}