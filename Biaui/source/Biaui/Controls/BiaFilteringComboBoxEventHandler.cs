using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Biaui.Internals;
using Jewelry.Memory;

namespace Biaui.Controls;

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

        wordTextBox.Dispatcher?.BeginInvoke(new Action(() => wordTextBox.Focus()));
    }

    [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
    private void WordTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Down &&
            e.Key != Key.Up)
            return;

        var textBox = (FrameworkElement) sender;
        var sv = textBox.Parent.Descendants().OfType<ScrollViewer>().First();

        if (e.Key == Key.Down)
            sv.ScrollToHome();
        else
            sv.ScrollToBottom();

        // ※.VirtualizingStackPanelなためスクロールを済ませてからアイテムを探しに行く
        sv.Dispatcher?.BeginInvoke(
            DispatcherPriority.Input,
            new Action(() =>
            {
                using var items = textBox.Parent.Descendants().OfType<ComboBoxItem>().ToTempBuffer(512);

                if (items.Length == 0)
                    return;

                var selectedItem = items.FirstOrDefault(x => x.IsSelected);
                var isDown = e.Key == Key.Down;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (selectedItem is null)
                {
                    var item = isDown
                        ? items.FirstOrDefault()
                        : items.LastOrDefault();

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (item is not null)
                        item.IsSelected = true;
                }
                else
                {
                    var selectedItemIndex = items.IndexOf(selectedItem);
                    var nextIndex = selectedItemIndex + (isDown ? +1 : -1);

                    nextIndex = Math.Max(nextIndex, 0);
                    nextIndex = Math.Min(nextIndex, items.Length - 1);

                    var nextItem = items[nextIndex] ?? throw new InvalidOperationException();
                    nextItem.IsSelected = true;
                }

                textBox.Focus();
            }));
    }

    private void Popup_OnClosed(object sender, EventArgs e)
    {
        var popup = (FrameworkElement) sender;
        var parent = popup.GetParent<BiaFilteringComboBox>();

        if (parent is null)
            return;

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
        var parent = popup.GetParent<BiaFilteringComboBox>() ?? throw new InvalidOperationException();

        var descriptor = DependencyPropertyDescriptor.FromProperty(BiaFilteringComboBox.FilterWordsProperty,
            typeof(BiaFilteringComboBox));

        descriptor.AddValueChanged(parent, UpdateClearButtonHandler);
    }

    private void Popup_OnUnloaded(object sender, RoutedEventArgs e)
    {
        var popup = (Popup) sender;
        var parent = popup.GetParent<BiaFilteringComboBox>() ?? throw new InvalidOperationException();

        var descriptor = DependencyPropertyDescriptor.FromProperty(BiaFilteringComboBox.FilterWordsProperty,
            typeof(BiaFilteringComboBox));

        descriptor.RemoveValueChanged(parent, UpdateClearButtonHandler);
    }

    private static void UpdateClearButtonHandler(object? sender, EventArgs e)
    {
        if (sender is FrameworkElement popup)
            UpdateClearButton(popup);
    }

    private static void UpdateClearButton(FrameworkElement popup)
    {
        var parent = popup.GetParent<BiaFilteringComboBox>();
        var clearButton = popup.Descendants().OfType<Button>().FirstOrDefault();

        if (clearButton != null)
            clearButton.IsEnabled = string.IsNullOrEmpty(parent?.FilterWords) == false;
    }
}