using System;
using System.Globalization;
using System.Windows.Data;

namespace Biaui.Controls.Converters
{
    public class DoubleColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DoubleColor item))
                return null!;

            return Caches.GetSolidColorBrush(item.Color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}