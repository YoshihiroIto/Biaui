using System;
using System.Windows;
using System.Windows.Controls;
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

                if (maxChildDesiredWidth < childDesiredSize.Width)
                    maxChildDesiredWidth = childDesiredSize.Width;

                if (maxChildDesiredHeight < childDesiredSize.Height)
                    maxChildDesiredHeight = childDesiredSize.Height;
            }

            return new Size(maxChildDesiredWidth * _columns, maxChildDesiredHeight * _rows);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var xIndex = 0;
            var yIndex = 0;

            var baseChildWidth = Math.Floor(arrangeSize.Width / _columns);
            var childHeight = Math.Floor(arrangeSize.Height / _rows);

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
    }
}