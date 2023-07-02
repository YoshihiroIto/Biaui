using System;
using System.Globalization;
using System.Windows.Data;

namespace Biaui.Controls.Converters;

public class BoolInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            null => Binding.DoNothing,
            bool b => Boxes.Bool(b == false),
            _ => Binding.DoNothing
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            null => Binding.DoNothing,
            bool b => Boxes.Bool(b == false),
            _ => Binding.DoNothing
        };
    }
}