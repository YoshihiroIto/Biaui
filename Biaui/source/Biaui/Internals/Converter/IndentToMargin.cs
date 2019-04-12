using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Biaui.Internals.Converter
{
    internal class IndentToMargin : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Boxes.Thickness0;

            if (!(values[0] is TreeViewItem item))
                return Boxes.Thickness0;

            var length = 19.0;

            if (values.Length == 2)
                if (values[1] is double)
                    length = (double) values[1];

            return new Thickness(length * item.GetDepth(), 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}