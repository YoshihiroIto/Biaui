using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls;

public partial class BiaEditableTextBlockEventHandler
{
    private void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox) sender;

        textBox.CaptureMouse();
        textBox.Focus();
    }

    private string? _startText;

    private void TextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var parent = ((BiaTextBlock)sender).GetParent<BiaEditableTextBlock>();

        Debug.Assert(parent != null);

        if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
        {
            parent.IsEditing = true;
            _startText = parent.Text;
        }

        var listItem = parent.GetParent<ListBoxItem>();
        if (listItem != null)
            listItem.IsSelected = true;
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox) sender;
        var parent = textBox.GetParent<BiaEditableTextBlock>();

        Debug.Assert(parent != null);

        FinishEditing(parent, textBox);
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (TextBox) sender;
        var parent = textBox.GetParent<BiaEditableTextBlock>();

        Debug.Assert(parent != null);

        switch (e.Key)
        {
            case Key.Tab:
                parent.Text = textBox.Text;
                FinishEditing(parent, textBox);
                e.Handled = true;
                break;

            case Key.Return:
                parent.Text = textBox.Text;
                FinishEditing(parent, textBox);
                e.Handled = true;
                break;

            case Key.Escape:
                parent.Text = _startText;
                FinishEditing(parent, textBox);
                e.Handled = true;
                break;
        }
    }

    private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var textBox = (TextBox) sender;

        var rounder = new LayoutRounder(textBox);
            
        // 自コントロール上であれば、終了させない
        var pos = e.GetPosition(textBox);
        var rect = rounder.RoundRenderRectangle(false);
        if (rect.Contains(pos))
            return;

        var parent = textBox.GetParent<BiaEditableTextBlock>();

        Debug.Assert(parent != null);

        FinishEditing(parent, textBox);
    }

    private static void FinishEditing(BiaEditableTextBlock parent, TextBox textBox)
    {
        textBox.ReleaseMouseCapture();

        parent.IsEditing = false;
    }
}