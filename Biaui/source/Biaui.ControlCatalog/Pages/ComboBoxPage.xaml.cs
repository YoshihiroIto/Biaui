using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Biaui.ControlCatalog.Pages
{
    public partial class ComboBoxPage
    {
        public string[] Items => new[]
        {
            "Africa",
            "Asia",
            "Europe",
            "North America",
            "South America",
            "Antarctica",
            "Australia"
        };

        #region SelectedItem

        public string SelectedItem
        {
            get => (string) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(string),
                typeof(ComboBoxPage));

        #endregion

        public Fruits[] EnumItems => Enum.GetValues(typeof(Fruits)).Cast<Fruits>().ToArray();
        
        #region SelectedEnumItem

        public Fruits SelectedEnumItem
        {
            get => (Fruits) GetValue(SelectedEnumItemsProperty);
            set => SetValue(SelectedEnumItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedEnumItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedEnumItem),
                typeof(Fruits),
                typeof(ComboBoxPage));

        #endregion

        public ComboBoxPage()
        {
            Name = "ComboBox";
            InitializeComponent();

            SelectedItem = Items[0];
            SelectedEnumItem = EnumItems[0];
        }
    }

    public enum Fruits
    {
        Apple,
        Banana,
        Orange,
        Lemon
    };

    public class FruitsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Fruits fruits))
                return null;

            // ReSharper disable StringLiteralTypo
            return fruits switch
            {
                Fruits.Apple => "Pomme",
                Fruits.Banana => "Banane",
                Fruits.Orange => "Orange",
                Fruits.Lemon => "Citron",
                _ => throw new ArgumentOutOfRangeException()
            };
            // ReSharper restore StringLiteralTypo
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}