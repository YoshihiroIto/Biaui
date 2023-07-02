using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Biaui.Internals;
using Jewelry.Collections;

namespace Biaui.Controls.Converters;

public class IndentToMarginConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null)
            return Boxes.Thickness0;

        if (!(values[0] is TreeViewItem item))
            return Boxes.Thickness0;

        var length = 19.0;

        if (values.Length == 2)
            if (values[1] is double v)
                length = v;

        var k = length * item.GetDepth();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (k == 0)
            return Boxes.Thickness0;

        if (_thicknessCache.TryGetValue(k, out var result) == false)
        {
            result = new Thickness(k, 0, 0, 0);

            _thicknessCache.Add(k, result);
        }

        return result;
    }

    private static readonly LruCache<double, object> _thicknessCache = new LruCache<double, object>(8);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}