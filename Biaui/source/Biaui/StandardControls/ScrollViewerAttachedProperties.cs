using System.Windows;
using Biaui.Internals;

namespace Biaui.StandardControls
{
    public class ScrollViewerAttachedProperties
    {
        #region HorizontalScrollBarOpacity

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

        #endregion

        #region VerticalScrollBarOpacity

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

        #endregion

        #region IsLeftVerticalScrollBar

        public static readonly DependencyProperty IsLeftVerticalScrollBarProperty =
            DependencyProperty.RegisterAttached(
                "IsLeftVerticalScrollBar",
                typeof(bool),
                typeof(ScrollViewerAttachedProperties),
                new FrameworkPropertyMetadata(Boxes.BoolFalse));

        public static bool GetIsLeftVerticalScrollBar(DependencyObject target)
        {
            return (bool) target.GetValue(IsLeftVerticalScrollBarProperty);
        }

        public static void SetIsLeftVerticalScrollBar(DependencyObject target, bool value)
        {
            target.SetValue(IsLeftVerticalScrollBarProperty, Boxes.Bool(value));
        }

        #endregion
    }
}