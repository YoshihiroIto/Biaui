using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.StandardControls
{
    public partial class ExpanderEventHandler
    {
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };

            (sender as UIElement)?.RaiseEvent(e2);
        }

        private void Frame_OnLoaded(object sender, RoutedEventArgs e)
        {
            var c = (StackPanel) sender;
            var expander = c.GetParent<Expander>();

            c.Tag = expander.IsExpanded ? Boxes.Double1 : Boxes.Double0;
        }
    }
}