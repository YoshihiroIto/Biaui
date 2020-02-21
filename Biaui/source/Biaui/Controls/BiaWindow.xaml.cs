using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaWindow : Window
    {
        #region Menu

        public Menu? Menu
        {
            get => _Menu;
            set
            {
                if (value != _Menu)
                    SetValue(MenuProperty, value);
            }
        }

        private Menu? _Menu;

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

        #region MenuRight

        public UIElement? MenuRight
        {
            get => _MenuRight;
            set
            {
                if (value != _MenuRight)
                    SetValue(MenuRightProperty, value);
            }
        }

        private UIElement? _MenuRight;

        public static readonly DependencyProperty MenuRightProperty =
            DependencyProperty.Register(
                nameof(MenuRight),
                typeof(UIElement),
                typeof(BiaWindow),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._MenuRight = (UIElement) e.NewValue;
                    }));

        #endregion
        
        #region HamburgerMenuItem
        
        public BiaHamburgerMenu? HamburgerMenu
        {
            get => _hamburgerMenu;
            set
            {
                if (value != _hamburgerMenu)
                    SetValue(HamburgerMenuProperty, value);
            }
        }
        
        private BiaHamburgerMenu? _hamburgerMenu;
        
        public static readonly DependencyProperty HamburgerMenuProperty =
            DependencyProperty.Register(
                nameof(HamburgerMenu),
                typeof(BiaHamburgerMenu),
                typeof(BiaWindow),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._hamburgerMenu = (BiaHamburgerMenu)e.NewValue;
                    }));

        #endregion

        #region CloseButtonBehavior

        public BiaWindowCloseButtonBehavior CloseButtonBehavior
        {
            get => _CloseButtonBehavior;
            set
            {
                if (value != _CloseButtonBehavior)
                    SetValue(CloseButtonBehaviorProperty, Boxes.WindowCloseButton(value));
            }
        }

        private BiaWindowCloseButtonBehavior _CloseButtonBehavior = BiaWindowCloseButtonBehavior.Normal;

        public static readonly DependencyProperty CloseButtonBehaviorProperty =
            DependencyProperty.Register(
                nameof(CloseButtonBehavior),
                typeof(BiaWindowCloseButtonBehavior),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.WindowCloseButtonBehaviorNormal,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._CloseButtonBehavior = (BiaWindowCloseButtonBehavior) e.NewValue;
                    }));

        #endregion

        #region CloseButtonClickedCommand

        public ICommand? CloseButtonClickedCommand
        {
            get => _CloseButtonClickedCommand;
            set
            {
                if (value != _CloseButtonClickedCommand)
                    SetValue(CloseButtonClickedCommandProperty, value);
            }
        }

        private ICommand? _CloseButtonClickedCommand;

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

        #region IsVisibleMinimizeButton

        public bool IsVisibleMinimizeButton
        {
            get => _IsVisibleMinimizeButton;
            set
            {
                if (value != _IsVisibleMinimizeButton)
                    SetValue(IsVisibleMinimizeButtonProperty, Boxes.Bool(value));
            }
        }

        private bool _IsVisibleMinimizeButton = true;

        public static readonly DependencyProperty IsVisibleMinimizeButtonProperty =
            DependencyProperty.Register(
                nameof(IsVisibleMinimizeButton),
                typeof(bool),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._IsVisibleMinimizeButton = (bool) e.NewValue;
                    }));

        #endregion

        #region IsVisibleMaximizeButton

        public bool IsVisibleMaximizeButton
        {
            get => _IsVisibleMaximizeButton;
            set
            {
                if (value != _IsVisibleMaximizeButton)
                    SetValue(IsVisibleMaximizeButtonProperty, Boxes.Bool(value));
            }
        }

        private bool _IsVisibleMaximizeButton = true;

        public static readonly DependencyProperty IsVisibleMaximizeButtonProperty =
            DependencyProperty.Register(
                nameof(IsVisibleMaximizeButton),
                typeof(bool),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._IsVisibleMaximizeButton = (bool) e.NewValue;
                    }));

        #endregion

        #region IsVisibleNormalizeButton

        public bool IsVisibleNormalizeButton
        {
            get => _IsVisibleNormalizeButton;
            set
            {
                if (value != _IsVisibleNormalizeButton)
                    SetValue(IsVisibleNormalizeButtonProperty, Boxes.Bool(value));
            }
        }

        private bool _IsVisibleNormalizeButton = true;

        public static readonly DependencyProperty IsVisibleNormalizeButtonProperty =
            DependencyProperty.Register(
                nameof(IsVisibleNormalizeButton),
                typeof(bool),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._IsVisibleNormalizeButton = (bool) e.NewValue;
                    }));

        #endregion

        #region IsVisibleCloseButtonButton

        public bool IsVisibleCloseButtonButton
        {
            get => _IsVisibleCloseButtonButton;
            set
            {
                if (value != _IsVisibleCloseButtonButton)
                    SetValue(IsVisibleCloseButtonButtonProperty, Boxes.Bool(value));
            }
        }

        private bool _IsVisibleCloseButtonButton = true;

        public static readonly DependencyProperty IsVisibleCloseButtonButtonProperty =
            DependencyProperty.Register(
                nameof(IsVisibleCloseButtonButton),
                typeof(bool),
                typeof(BiaWindow),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._IsVisibleCloseButtonButton = (bool) e.NewValue;
                    }));

        #endregion

        #region Icon

        public new Brush Icon
        {
            get => _Icon;
            set
            {
                if (value != _Icon)
                    SetValue(IconProperty, value);
            }
        }

        private Brush _Icon = DefaultIcon;

        public new static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(Brush),
                typeof(BiaWindow),
                new PropertyMetadata(
                    DefaultIcon,
                    (s, e) =>
                    {
                        var self = (BiaWindow) s;
                        self._Icon = (Brush) e.NewValue;
                    }));

        #endregion

        private static Brush DefaultIcon =>
            _DefaultIcon ??= Caches.GetSolidColorBrush(new ByteColor(0xFF, 0x53, 0x7C, 0xCE));

        private static Brush? _DefaultIcon;

        public event EventHandler? CloseButtonClicked;

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

        internal void InvokeCloseButtonClicked()
        {
            CloseButtonClicked?.Invoke(this, EventArgs.Empty);

            CloseButtonClickedCommand?.ExecuteIfCan(null);
        }

        // https://stackoverflow.com/questions/29207331/wpf-window-with-custom-chrome-has-unwanted-outline-on-right-and-bottom
        protected void FixLayout()
        {
            void WindowSourceInitialized(object? sender, EventArgs e)
            {
                InvalidateMeasure();
                SourceInitialized -= WindowSourceInitialized;
            }

            SourceInitialized += WindowSourceInitialized;
        }
    }
}