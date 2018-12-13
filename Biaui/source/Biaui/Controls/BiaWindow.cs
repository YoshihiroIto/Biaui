using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaWindow : Window
    {
        #region Menu

        public Menu Menu
        {
            get => _Menu;
            set
            {
                if (value != _Menu)
                    SetValue(MenuProperty, value);
            }
        }

        private Menu _Menu;

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(Menu), typeof(BiaWindow),
                new PropertyMetadata(
                    default(Menu),
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._Menu = (Menu) e.NewValue;
                    }));

        #endregion

        static BiaWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaWindow),
                new FrameworkPropertyMetadata(typeof(BiaWindow)));

            // -------------------------------------
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(Boxes.Int0));

            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(Boxes.IntMax));

            ToolTipService.BetweenShowDelayProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(Boxes.Int0));
        }
    }
}