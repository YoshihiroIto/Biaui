using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Biaui.Controls.Internals
{
    internal class SystemButton : Button
    {
        #region BiaWindowAction

        public BiaWindowAction WindowAction
        {
            get => _WindowAction;
            set
            {
                if (value != _WindowAction)
                    SetValue(WindowActionProperty, Boxes.WindowAction(value));
            }
        }

        private BiaWindowAction _WindowAction;

        public static readonly DependencyProperty WindowActionProperty =
            DependencyProperty.Register(nameof(WindowAction), typeof(BiaWindowAction), typeof(SystemButton),
                new PropertyMetadata(
                    Boxes.WindowActionNone,
                    (s, e) =>
                    {
                        var self = (SystemButton) s;
                        self._WindowAction = (BiaWindowAction) e.NewValue;
                    }));

        #endregion

        #region IsVisibleButton
        
        public bool IsVisibleButton
        {
            get => _IsVisibleButton;
            set
            {
                if (value != _IsVisibleButton)
                    SetValue(IsVisibleButtonProperty, Boxes.Bool(value));
            }
        }
        
        private bool _IsVisibleButton = true;
        
        public static readonly DependencyProperty IsVisibleButtonProperty =
            DependencyProperty.Register(
                nameof(IsVisibleButton),
                typeof(bool),
                typeof(SystemButton),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (SystemButton) s;
                        self._IsVisibleButton = (bool)e.NewValue;

                        self.MakeVisibility();
                    }));
        
        #endregion

        static SystemButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SystemButton),
                new FrameworkPropertyMetadata(typeof(SystemButton)));
        }

        private BiaWindow? _parentWindow;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _parentWindow = (BiaWindow)Window.GetWindow(this);

            if (_parentWindow != null)
                _parentWindow.StateChanged += ParentWindowOnStateChanged;

            MakeVisibility();
        }

        private void ParentWindowOnStateChanged(object? sender, EventArgs e)
        {
            MakeVisibility();
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (_parentWindow == null)
                return;

            switch (WindowAction)
            {
                case BiaWindowAction.None:
                    break;

                case BiaWindowAction.Active:
                    _parentWindow.Activate();
                    break;

                case BiaWindowAction.Close:
                    if (_parentWindow.CloseButtonBehavior == BiaWindowCloseButtonBehavior.Normal)
                        _parentWindow.Close();
                    
                    _parentWindow.InvokeCloseButtonClicked();

                    break;

                case BiaWindowAction.Maximize:
                    _parentWindow.WindowState = WindowState.Maximized;
                    break;

                case BiaWindowAction.Minimize:
                    _parentWindow.WindowState = WindowState.Minimized;
                    break;

                case BiaWindowAction.Normalize:
                    _parentWindow.WindowState = WindowState.Normal;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MakeVisibility()
        {
            if (IsVisibleButton == false)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            if (_parentWindow == null)
                return;

            Visibility = WindowAction switch
            {
                BiaWindowAction.Maximize => _parentWindow.WindowState != WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed,
                BiaWindowAction.Minimize => _parentWindow.WindowState != WindowState.Minimized ? Visibility.Visible : Visibility.Collapsed,
                BiaWindowAction.Normalize => _parentWindow.WindowState != WindowState.Normal ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility
            };
        }
    }
}