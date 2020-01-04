using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.StandardControls
{
    public partial class ScrollViewerEventHandler
    {
        private const double Response = 150.0;
        private const double MaxOpacity = 0.6;
        private const double BarWidth = 8.0;

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var self = ((FrameworkElement) sender).GetParent<ScrollViewer>();

            var pos = e.GetPosition(self);
            var posX = pos.X;
            var posY = pos.Y;

            var width = (1.0, self.ActualWidth - BarWidth).Max();
            var height = (1.0, self.ActualHeight - BarWidth).Max();

            if (ScrollViewerAttachedProperties.GetIsLeftVerticalScrollBar(self))
                posX = width - posX;

            var xr = (Response, width).Min();
            var yr = (Response, height).Min();

            var xir = width - xr;
            var yir = height - yr;

            var xd = NumberHelper.Clamp01((posX - xir) / xr);
            var yd = NumberHelper.Clamp01((posY - yir) / yr);

            var xo = xd * MaxOpacity;
            var yo = yd * MaxOpacity;

            ScrollViewerAttachedProperties.SetVerticalScrollBarOpacity(self, xo);
            ScrollViewerAttachedProperties.SetHorizontalScrollBarOpacity(self, yo);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var self = ((FrameworkElement) sender).GetParent<ScrollViewer>();

            ScrollViewerAttachedProperties.SetVerticalScrollBarOpacity(self, 0.0);
            ScrollViewerAttachedProperties.SetHorizontalScrollBarOpacity(self, 0.0);
        }
    }
}