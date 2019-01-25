using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Biaui.Controls.Mock.Control.Behavior
{
    public class WindowDisposeDataContextOnClosedBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Closed += AssociatedObjectOnClosed;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Closed -= AssociatedObjectOnClosed;

            base.OnDetaching();
        }

        private void AssociatedObjectOnClosed(object sender, EventArgs eventArgs)
        {
            (AssociatedObject.DataContext as IDisposable)?.Dispose();
        }
    }
}