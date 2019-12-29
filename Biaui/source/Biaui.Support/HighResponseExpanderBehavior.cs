using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls;
using Biaui.Internals;

namespace Biaui
{
    public class HighResponseExpanderBehavior : Microsoft.Xaml.Behaviors.Behavior<Expander>
    {
        #region IgnoreControls

        public IEnumerable? IgnoreControls
        {
            get => _IgnoreControls;
            set
            {
                if (!Equals(value, _IgnoreControls))
                    SetValue(IgnoreControlsProperty, value);
            }
        }

        private IEnumerable? _IgnoreControls = new[] {typeof(CheckBox), typeof(BiaCheckBox)};

        public static readonly DependencyProperty IgnoreControlsProperty =
            DependencyProperty.Register(
                nameof(IgnoreControls),
                typeof(IEnumerable),
                typeof(HighResponseExpanderBehavior),
                new PropertyMetadata(
                    new[] {typeof(CheckBox), typeof(BiaCheckBox)},
                    (s, e) =>
                    {
                        var self = (HighResponseExpanderBehavior) s;
                        self._IgnoreControls = (IEnumerable) e.NewValue;
                    }));

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseDown += AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.PreviewMouseMove += AssociatedObjectOnPreviewMouseMove;
            AssociatedObject.PreviewMouseUp += AssociatedObjectOnPreviewMouseUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseDown -= AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.PreviewMouseMove -= AssociatedObjectOnPreviewMouseMove;
            AssociatedObject.PreviewMouseUp -= AssociatedObjectOnPreviewMouseUp;
        }

        private static Expander? _leader;
        private static bool _leaderIsExpanded;

        private void AssociatedObjectOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var (isOn, hitTestResult) = IsOnIgnoreControls();
            if (isOn)
                return;

            var expander = (hitTestResult?.VisualHit as FrameworkElement)?.GetParent<Expander>();
            if (expander == null)
                return;

            if (IsOnHeader() == false)
                return;

            AssociatedObject.IsExpanded = !AssociatedObject.IsExpanded;
            e.Handled = true;

            _leader = AssociatedObject;
            _leaderIsExpanded = AssociatedObject.IsExpanded;

            AssociatedObject.CaptureMouse();
        }

        private void AssociatedObjectOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (AssociatedObject != _leader)
                return;

            var (isOn, hitTestResult) = IsOnIgnoreControls();
            if (isOn)
                return;

            if (IsOnHeader() == false)
                return;

            var otherExpander = (hitTestResult?.VisualHit as FrameworkElement)?.GetParent<Expander>();
            if (otherExpander == null)
                return;

            if (otherExpander.IsEnabled)
                otherExpander.IsExpanded = _leaderIsExpanded;
        }

        private void AssociatedObjectOnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _leader = null;

            AssociatedObject.ReleaseMouseCapture();
        }

        private (bool isOn, HitTestResult? hitTestResult) IsOnIgnoreControls()
        {
            var window = AssociatedObject.GetParent<Window>();
            if (window == null)
                return (false, null);

            var hitTestResult = VisualTreeHelper.HitTest(window, Mouse.GetPosition(window));

            if (IgnoreControls != null)
            {
                foreach (Type? type in IgnoreControls)
                {
                    var c = (hitTestResult?.VisualHit as FrameworkElement)?.GetParent(type!);
                    if (c != null)
                        return (true, hitTestResult);
                }
            }

            return (false, hitTestResult);
        }

        private bool IsOnHeader()
        {
            var window = AssociatedObject.GetParent<Window>();
            if (window == null)
                return false;

            var hitTestResult = VisualTreeHelper.HitTest(window, Mouse.GetPosition(window));

            var header = (hitTestResult?.VisualHit as FrameworkElement)?.GetParent<ToggleButton>();
            if (header == null)
                return false;

            if (header.Name != "HeaderSite")
                return false;

            return true;
        }
    }
}