using System.Windows;
using Biaui.Internals;

namespace Biaui.StandardControls
{
    public class ScrollViewerAttachedProperties
    {
        public static readonly DependencyProperty HorizontalScrollBarOpacityProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalScrollBarOpacity",
                typeof(double),
                typeof(ScrollViewerAttachedProperties),
                new FrameworkPropertyMetadata(Boxes.Double0));

        public static double GetHorizontalScrollBarOpacityScrollBar(DependencyObject target)
        {
            return (double) target.GetValue(HorizontalScrollBarOpacityProperty);
        }

        public static void SetHorizontalScrollBarOpacity(DependencyObject target, double value)
        {
            target.SetValue(HorizontalScrollBarOpacityProperty, Boxes.Double(value));
        }

        public static readonly DependencyProperty VerticalScrollBarOpacityProperty =
            DependencyProperty.RegisterAttached(
                "VerticalScrollBarOpacity",
                typeof(double),
                typeof(ScrollViewerAttachedProperties),
                new FrameworkPropertyMetadata(Boxes.Double0));

        public static double GetVerticalScrollBarOpacity(DependencyObject target)
        {
            return (double) target.GetValue(VerticalScrollBarOpacityProperty);
        }

        public static void SetVerticalScrollBarOpacity(DependencyObject target, double value)
        {
            target.SetValue(VerticalScrollBarOpacityProperty, Boxes.Double(value));
        }
    }
}