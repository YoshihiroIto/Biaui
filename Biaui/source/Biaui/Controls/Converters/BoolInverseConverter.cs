using System;
using System.Globalization;
using System.Windows.Data;

namespace Biaui.Controls.Converters
{
    public class BoolInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Binding.DoNothing;

            if (value is bool b)
                return Boxes.Bool(b == false);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Binding.DoNothing;

            if (value is bool b)
                return Boxes.Bool(b == false);

            return Binding.DoNothing;
        }
    }
}