using System;
using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;

namespace Biaui.Controls.Internals
{
    internal class SystemButton : Button
    {
        #region WindowAction

        public WindowAction WindowAction
        {
            get => _WindowAction;
            set
            {
                if (value != _WindowAction)
                    SetValue(WindowActionProperty, value);
            }
        }

        private WindowAction _WindowAction;

        public static readonly DependencyProperty WindowActionProperty =
            DependencyProperty.Register(nameof(WindowAction), typeof(WindowAction), typeof(SystemButton),
                new PropertyMetadata(
                    default(WindowAction),
                    (s, e) =>
                    {
                        var self = (SystemButton) s;
                        self._WindowAction = (WindowAction) e.NewValue;
                    }));

        #endregion

        static SystemButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SystemButton),
                new FrameworkPropertyMetadata(typeof(SystemButton)));
        }

        private Window _parentWindow;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _parentWindow = Window.GetWindow(this);

            if (_parentWindow != null)
                _parentWindow.StateChanged += ParentWindowOnStateChanged;

            MakeVisibility();
        }

        private void ParentWindowOnStateChanged(object sender, EventArgs e)
        {
            MakeVisibility();
        }

        protected override void OnClick()
        {
            base.OnClick();

            switch (WindowAction)
            {
                case WindowAction.None:
                    break;

                case WindowAction.Active:
                    _parentWindow.Activate();
                    break;

                case WindowAction.Close:
                    _parentWindow.Close();
                    break;

                case WindowAction.Maximize:
                    _parentWindow.WindowState = WindowState.Maximized;
                    break;

                case WindowAction.Minimize:
                    _parentWindow.WindowState = WindowState.Minimized;
                    break;

                case WindowAction.Normalize:
                    _parentWindow.WindowState = WindowState.Normal;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MakeVisibility()
        {
            switch (WindowAction)
            {
                case WindowAction.Maximize:
                    Visibility = _parentWindow.WindowState != WindowState.Maximized
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;

                case WindowAction.Minimize:
                    Visibility = _parentWindow.WindowState != WindowState.Minimized
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;

                case WindowAction.Normalize:
                    Visibility = _parentWindow.WindowState != WindowState.Normal
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
            }
        }
    }
}