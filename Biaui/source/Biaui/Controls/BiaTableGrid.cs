using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biaui.Controls
{
    public class BiaTableGrid : Grid
    {
        protected static DependencyProperty GetMarginProperty(DependencyObject obj)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null && dpd.Name == nameof(Margin))
                    return dpd.DependencyProperty;
            }

            return null;
        }

        #region Spacing

        public Size Spacing
        {
            get => _Spacing;
            set
            {
                if (value != _Spacing)
                    SetValue(SpacingProperty, value);
            }
        }

        private Size _Spacing;

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                nameof(Spacing),
                typeof(Size),
                typeof(BiaTableGrid),
                new PropertyMetadata(
                    default(Size),
                    (s, e) =>
                    {
                        var self = (BiaTableGrid) s;
                        self._Spacing = (Size) e.NewValue;

                        self.UpdateColumns();
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
                typeof(BiaTableGrid),
                new FrameworkPropertyMetadata(
                    Boxes.Int0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    (s, e) =>
                    {
                        var self = (BiaTableGrid) s;
                        self._Columns = (int) e.NewValue;

                        self.UpdateColumns();
                    }));

        #endregion

        private void UpdateColumns()
        {
            var childCount = VisualTreeHelper.GetChildrenCount(this);

            {
                while (ColumnDefinitions.Count < childCount)
                {
                    var cd = new ColumnDefinition
                    {
                        Width = new GridLength(0, GridUnitType.Auto)
                    };
                    ColumnDefinitions.Add(cd);
                }

                while (ColumnDefinitions.Count > childCount)
                    ColumnDefinitions.RemoveAt(ColumnDefinitions.Count - 1);
            }

            {
                var rows = childCount / Columns;

                while (RowDefinitions.Count < rows)
                {
                    var rd = new RowDefinition
                    {
                        Height = new GridLength(0, GridUnitType.Star)
                    };
                    RowDefinitions.Add(rd);
                }

                while (RowDefinitions.Count > rows)
                    RowDefinitions.RemoveAt(RowDefinitions.Count - 1);
            }

            {
                var columnCount = 0;
                var rowCount = 0;
                var isLastRow = false;

                var rows = childCount / Columns;

                for (var i = 0; i != childCount; ++i)
                {
                    var child = (FrameworkElement) VisualTreeHelper.GetChild(this, i);
                    
                    SetColumn(child, columnCount);
                    SetRow(child, rowCount);

                    var px = columnCount == Columns - 1
                        ? 0d
                        : Spacing.Width;
                    var py = isLastRow
                        ? 0d
                        : Spacing.Height;
                    
                    child.Margin = new Thickness(0d, 0d, px, py);

                    ++columnCount;

                    if (columnCount == Columns)
                    {
                        columnCount = 0;
                        ++rowCount;

                        isLastRow = rowCount == rows - 1;
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (_isRequestUpdateColumns)
            {
                UpdateColumns();
                _isRequestUpdateColumns = false;
            }

            return base.ArrangeOverride(arrangeSize);
        }

        private bool _isRequestUpdateColumns;

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            _isRequestUpdateColumns = true;

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}