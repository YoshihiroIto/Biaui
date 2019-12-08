using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Biaui.Internals.Converter
{
    internal class ExpanderMultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)values[0];
            var margin = (Thickness)values[1];
            var scale = (double)values[2];

            var result = (height + margin.Top + margin.Bottom) * scale;

            return Boxes.Double(result);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}