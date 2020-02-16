using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;
using System.Windows.Controls.Primitives;
using Application = System.Windows.Application;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using ToolTip = System.Windows.Controls.ToolTip;

namespace Biaui
{
    public class ScrollViewerMiddleButtonDragBehavior : Microsoft.Xaml.Behaviors.Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseDown += AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.PreviewMouseUp += AssociatedObjectOnPreviewMouseUp;
            AssociatedObject.PreviewMouseMove += AssociatedObjectOnPreviewMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseDown -= AssociatedObjectOnPreviewMouseDown;
            AssociatedObject.PreviewMouseUp -= AssociatedObjectOnPreviewMouseUp;
            AssociatedObject.PreviewMouseMove -= AssociatedObjectOnPreviewMouseMove;
        }

        private void AssociatedObjectOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled == false)
                return;

            if (_isInDragging)
                CancelDrag();


            else if (e.ChangedButton == MouseButton.Middle &&
                     e.ButtonState == MouseButtonState.Pressed)
            {
                if (_isInDragging)
                    return;

                _isInDragging = true;

                // ReSharper disable once SuspiciousTypeConversion.Global
                _previousPos = e.GetPosition(ParentControl);

                var scrollBars = AssociatedObject
                    .Descendants()
                    .OfType<ScrollBar>()
                    .Where(x => x.Visibility == Visibility.Visible);

                var v = 0;
                foreach (var scrollBar in scrollBars)
                {
                    if (scrollBar.Orientation == Orientation.Horizontal) v |= 1;
                    else /*if (scrollBar.Orientation == Orientation.Vertical)*/ v |= 2;
                }

                _cursor = AssociatedObject.Cursor;

                AssociatedObject.Cursor = v switch
                {
                    0 => Cursors.Arrow,
                    1 => Cursors.SizeWE,
                    2 => Cursors.SizeNS,
                    3 => Cursors.SizeAll
                };

                AssociatedObject.CaptureMouse();

                // Disable ToolTip
                if (_disableToolTipStyle == null)
                {
                    _disableToolTipStyle = new Style(typeof(ToolTip));
                    _disableToolTipStyle.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
                    _disableToolTipStyle.Seal();
                }

                foreach (Window? window in Application.Current.Windows)
                    window?.Resources.Add(typeof(ToolTip), _disableToolTipStyle);

                e.Handled = true;
            }
        }

        private void AssociatedObjectOnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled == false)
                return;

            if (e.ChangedButton == MouseButton.Middle &&
                e.ButtonState == MouseButtonState.Released)
            {
                CancelDrag();
                e.Handled = true;
            }
        }

        private void AssociatedObjectOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (IsEnabled == false)
                return;

            if (_isInDragging == false)
                return;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var currentPos = e.GetPosition(ParentControl);
            var diff = currentPos - _previousPos;

            AssociatedObject.ScrollToVerticalOffset(AssociatedObject.VerticalOffset - diff.Y);
            AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - diff.X);

            _previousPos = currentPos;

            e.Handled = true;
        }

        private void CancelDrag()
        {
            _isInDragging = false;

            AssociatedObject.ReleaseMouseCapture();
            AssociatedObject.Cursor = _cursor;
            _cursor = null;

            // Enable ToolTip
            foreach (Window? window in Application.Current.Windows)
                window?.Resources.Remove(typeof(ToolTip));
        }

        private bool IsEnabled =>
            !(AssociatedObject.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled &&
              AssociatedObject.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);

        // ReSharper disable once SuspiciousTypeConversion.Global
        private IInputElement? ParentControl => AssociatedObject.Parent as IInputElement;

        private bool _isInDragging;
        private Point _previousPos;
        private Cursor? _cursor;

        private static Style? _disableToolTipStyle;
    }
}