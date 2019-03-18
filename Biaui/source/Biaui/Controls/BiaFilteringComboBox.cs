using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaFilteringComboBox : ComboBox
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
                        self._FilterWord = (string) e.NewValue;
                    }));

        #endregion

        #region IsEnableMouseWheel

        public bool IsMouseWheelEnabled
        {
            get => _isMouseWheelEnabled;
            set
            {
                if (value != _isMouseWheelEnabled)
                    SetValue(IsMouseWheelEnabledProperty, value);
            }
        }

        private bool _isMouseWheelEnabled = true;

        public static readonly DependencyProperty IsMouseWheelEnabledProperty =
            DependencyProperty.Register(
                nameof(IsMouseWheelEnabled),
                typeof(bool),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._isMouseWheelEnabled = (bool) e.NewValue;
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

            if (IsMouseWheelEnabled)
                return;

            e.Handled = true;

            if (IsDropDownOpen)
                return;

            var parentScrollViewer = this.GetParent<ScrollViewer>();
            if (parentScrollViewer == null)
                return;

            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = this
            };

            parentScrollViewer.RaiseEvent(args);
        }
    }
}