using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls
{
    // ReSharper disable once UnusedMember.Global
    public partial class BiaFilteringComboBoxEventHandler
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement) sender;
            var wordTextBox = (TextBox) button.Tag;

            wordTextBox.Text = "";
            wordTextBox.Focus();
        }

        private void ClearButton_OnLoaded(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement) sender;
            var wordTextBox = (TextBox) button.Tag;

            wordTextBox.Dispatcher.BeginInvoke(new Action(() => wordTextBox.Focus()));
        }

        private void WordTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down &&
                e.Key != Key.Up)
                return;

            var button = (FrameworkElement) sender;
            var sv = button.Parent.Descendants<ScrollViewer>().First();

            if (e.Key == Key.Down)
                sv.ScrollToHome();
            else
                sv.ScrollToBottom();

            // ※.VirtualizingStackPanelなためスクロールを済ませてからアイテムを探しに行く
            sv.Dispatcher.BeginInvoke(new Action(() =>
            {
                var content =
                    e.Key == Key.Down
                        ? button.Parent.Descendants<ComboBoxItem>().FirstOrDefault()
                        : button.Parent.Descendants<ComboBoxItem>().LastOrDefault();

                content?.Focus();
            }));
        }

        private void Popup_OnClosed(object sender, EventArgs e)
        {
            var popup = (FrameworkElement) sender;
            var parent = popup.GetParent<BiaFilteringComboBox>();

            parent.FilterWords = "";

            var expression = parent.GetBindingExpression(Selector.SelectedItemProperty);
            expression?.UpdateTarget();
        }

        private void Popup_OnOpened(object sender, EventArgs e)
        {
            var popup = (FrameworkElement) sender;

            UpdateClearButton(popup);
        }

        private void Popup_OnLoaded(object sender, RoutedEventArgs e)
        {
            var popup = (Popup) sender;
            var parent = popup.GetParent<BiaFilteringComboBox>();

            var descriptor = DependencyPropertyDescriptor.FromProperty(BiaFilteringComboBox.FilterWordsProperty,
                typeof(BiaFilteringComboBox));

            descriptor.AddValueChanged(parent, UpdateClearButtonHandler);
        }

        private void Popup_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var popup = (Popup) sender;
            var parent = popup.GetParent<BiaFilteringComboBox>();

            var descriptor = DependencyPropertyDescriptor.FromProperty(BiaFilteringComboBox.FilterWordsProperty,
                typeof(BiaFilteringComboBox));

            descriptor.RemoveValueChanged(parent, UpdateClearButtonHandler);
        }

        private void UpdateClearButtonHandler(object sender, EventArgs e)
        {
            var popup = (FrameworkElement) sender;

            UpdateClearButton(popup);
        }

        private void UpdateClearButton(FrameworkElement popup)
        {
            var parent = popup.GetParent<BiaFilteringComboBox>();
            var clearButton = popup.Descendants<Button>().FirstOrDefault();

            if (clearButton != null)
                clearButton.IsEnabled = string.IsNullOrEmpty(parent.FilterWords) == false;
        }
    }
}