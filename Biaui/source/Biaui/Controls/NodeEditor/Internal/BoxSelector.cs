using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BoxSelector : FrameworkElement
    {
        private readonly MouseOperator _mouseOperator;

        static BoxSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxSelector),
                new FrameworkPropertyMetadata(typeof(BoxSelector)));
        }

        public BoxSelector(MouseOperator mouseOperator)
        {
            _mouseOperator = mouseOperator;

            mouseOperator.PreMouseLeftButtonUp += MouseOperatorOnMouseLeftButtonUp;
            mouseOperator.PreMouseMove += MouseOperatorOnMouseMove;
        }

        private void MouseOperatorOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseOperator.IsBoxSelect == false)
                return;

            Visibility = Visibility.Collapsed;
        }

        private void MouseOperatorOnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseOperator.IsBoxSelect == false)
                return;
            
            if (Visibility != Visibility.Visible)
                Visibility = Visibility.Visible;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var b = Caches.GetSolidColorBrush(Color.FromArgb(0x3F, 0x41, 0x69, 0xE1));
            var p = this.GetBorderPen(Color.FromArgb(0xFF, 0x41, 0x69, 0xE1));

            var r = FrameworkElementHelper.RoundLayoutRect(_mouseOperator.SelectionRect);
            dc.DrawRectangle(b, p, Unsafe.As<ImmutableRect, Rect>(ref r));
        }
    }
}