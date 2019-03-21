using System.Windows;
using System.Windows.Input;

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
    }
}