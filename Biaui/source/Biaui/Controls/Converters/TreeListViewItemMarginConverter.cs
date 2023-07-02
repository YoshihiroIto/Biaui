using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Biaui.Internals;

namespace Biaui.Controls.Converters;

public class TreeListViewItemMarginConverter : IValueConverter
{
    public double Length { get; set; }
    public bool IsFirstColumn { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is TreeViewItem item))
            return Boxes.Thickness0;

        var right = Length * (item.GetDepth() + 1.0);
        var left = IsFirstColumn ? -6.0 : right * -1.0;

        return new Thickness(left, 0, right, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}