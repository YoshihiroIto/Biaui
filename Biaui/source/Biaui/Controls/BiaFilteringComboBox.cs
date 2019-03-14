using System.Windows;

namespace Biaui.Controls
{
    public class BiaFilteringComboBox : System.Windows.Controls.ComboBox
    {
        #region FilterWord
        
        public string FilterWord
        {
            get => _FilterWord;
            set
            {
                if (value != _FilterWord)
                    SetValue(FilterWordProperty, value);
            }
        }
        
        private string _FilterWord;
        
        public static readonly DependencyProperty FilterWordProperty =
            DependencyProperty.Register(
                nameof(FilterWord),
                typeof(string),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._FilterWord = (string)e.NewValue;
                    }));
        
        #endregion

        static BiaFilteringComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaFilteringComboBox),
                new FrameworkPropertyMetadata(typeof(BiaFilteringComboBox)));
        }
    }
}