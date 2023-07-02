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
        if (value is not TreeViewItem item)
            return Boxes.Thickness0;

        var right = Length * (item.GetDepth() + 1d);
        var left = IsFirstColumn ? -6d : right * -1d;

        return new Thickness(left, 0d, right, 0d);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}