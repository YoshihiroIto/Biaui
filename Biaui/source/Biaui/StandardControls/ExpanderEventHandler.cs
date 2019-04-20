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

        private void ExpandSiteScrollView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sv = (ScrollViewer) sender;
            var expander = sv.GetParent<Expander>();

            sv.Tag = expander.IsExpanded ? 1.0 : 0.0;
        }
    }
}