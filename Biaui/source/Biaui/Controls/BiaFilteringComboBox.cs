using System.Windows;
using System.Windows.Input;
using Biaui.Internals;

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

        #region IsEnableMouseWheel
        
        public bool IsEnableMouseWheel
        {
            get => _IsEnableMouseWheel;
            set
            {
                if (value != _IsEnableMouseWheel)
                    SetValue(IsEnableMouseWheelProperty, value);
            }
        }
        
        private bool _IsEnableMouseWheel = true;
        
        public static readonly DependencyProperty IsEnableMouseWheelProperty =
            DependencyProperty.Register(
                nameof(IsEnableMouseWheel),
                typeof(bool),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._IsEnableMouseWheel = (bool)e.NewValue;
                    }));
        
        #endregion

        static BiaFilteringComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaFilteringComboBox),
                new FrameworkPropertyMetadata(typeof(BiaFilteringComboBox)));
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            e.Handled = IsEnableMouseWheel == false;
        }
    }
}