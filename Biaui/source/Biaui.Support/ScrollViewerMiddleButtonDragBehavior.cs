using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

                _cursor = AssociatedObject.Cursor;
                AssociatedObject.Cursor = Cursors.SizeNS;
                AssociatedObject.CaptureMouse();

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
        }

        private bool IsEnabled =>
            !(AssociatedObject.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled &&
              AssociatedObject.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);

        // ReSharper disable once SuspiciousTypeConversion.Global
        private IInputElement? ParentControl => AssociatedObject.Parent as IInputElement;

        private bool _isInDragging;
        private Point _previousPos;
        private Cursor? _cursor;
    }
}