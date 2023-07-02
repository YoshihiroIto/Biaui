using System;
using System.Globalization;
using System.Windows.Data;

namespace Biaui.StandardControls.Internal;

internal class ExpanderMultiplyConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var headerSiteHeight = (double)values[0];
        var expandSiteHeight = (double)values[1];
        var scale = (double)values[2];

        var result = headerSiteHeight + expandSiteHeight * scale;

        return Boxes.Double(result);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}