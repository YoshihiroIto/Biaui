using System;
using System.Globalization;
using System.Windows.Data;

namespace Biaui.Controls.Converters;

public class DoubleColorToBrushConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not DoubleColor item
            ? null
            : Caches.GetSolidColorBrush(item.ByteColor);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}