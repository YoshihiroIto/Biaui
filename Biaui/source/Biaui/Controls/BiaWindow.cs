using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        #region CloseButtonBehavior

        public WindowCloseButtonBehavior CloseButtonBehavior
        {
            get => _CloseButtonBehavior;
            set
            {
                if (value != _CloseButtonBehavior)
                    SetValue(CloseButtonBehaviorProperty, value);
            }
        }

        private WindowCloseButtonBehavior _CloseButtonBehavior = WindowCloseButtonBehavior.Normal;

        public static readonly DependencyProperty CloseButtonBehaviorProperty =
            DependencyProperty.Register(
                nameof(CloseButtonBehavior),
                typeof(WindowCloseButtonBehavior),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.WindowCloseButtonBehavior_Normal,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._CloseButtonBehavior = (WindowCloseButtonBehavior) e.NewValue;
                    }));

        #endregion

        #region CloseButtonClickedCommand

        public ICommand CloseButtonClickedCommand
        {
            get => _CloseButtonClickedCommand;
            set
            {
                if (value != _CloseButtonClickedCommand)
                    SetValue(CloseButtonClickedCommandProperty, value);
            }
        }

        private ICommand _CloseButtonClickedCommand;

        public static readonly DependencyProperty CloseButtonClickedCommandProperty =
            DependencyProperty.Register(
                nameof(CloseButtonClickedCommand),
                typeof(ICommand),
                typeof(BiaWindow),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._CloseButtonClickedCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        public event EventHandler CloseButtonClicked;

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

        public BiaWindow()
        {
            _CloseButtonClickedCommand = default(ICommand);
        }

        internal void InvokeCloseButtonClicked()
        {
            CloseButtonClicked?.Invoke(this, EventArgs.Empty);

            if (CloseButtonClickedCommand != null)
                if (CloseButtonClickedCommand.CanExecute(null))
                    CloseButtonClickedCommand.Execute(null);
        }
    }
}

public enum WindowCloseButtonBehavior
{
    Normal,
    DoNothing
}
