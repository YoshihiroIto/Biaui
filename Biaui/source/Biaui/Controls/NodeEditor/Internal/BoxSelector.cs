using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BoxSelector : Canvas
    {
        private readonly MouseOperator _mouseOperator;

        private readonly BoxSelectorBorder _left;
        private readonly BoxSelectorBorder _top;
        private readonly BoxSelectorBorder _right;
        private readonly BoxSelectorBorder _bottom;

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

            Children.Add(_left = new BoxSelectorBorder(false));
            Children.Add(_top = new BoxSelectorBorder(true));
            Children.Add(_right = new BoxSelectorBorder(false));
            Children.Add(_bottom = new BoxSelectorBorder(true));
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
            
            var rounder = new LayoutRounder(this);

            if (Visibility != Visibility.Visible)
                Visibility = Visibility.Visible;

            var r = rounder.RoundLayoutRect(_mouseOperator.SelectionRect);

            var borderWidth = rounder.RoundLayoutValue(2);

            SetLeft(_left, r.X);
            SetTop(_left, r.Y);
            _left.Width = borderWidth;
            _left.Height = r.Height;

            SetLeft(_right, r.X + r.Width);
            SetTop(_right, r.Y);
            _right.Width = borderWidth;
            _right.Height = r.Height;

            SetLeft(_top, r.X);
            SetTop(_top, r.Y);
            _top.Width = r.Width;
            _top.Height = borderWidth;

            SetLeft(_bottom, r.X);
            SetTop(_bottom, r.Y + r.Height);
            _bottom.Width = r.Width;
            _bottom.Height = borderWidth;
        }
    }

    internal class BoxSelectorBorder : FrameworkElement
    {
        private readonly bool _isVertical;

        internal BoxSelectorBorder(bool isVertical)
        {
            _isVertical = isVertical;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var rounder = new LayoutRounder(this);
            
            var pen = Caches.GetDashedPen(
                new ByteColor(0xFF, 0x41, 0x69, 0xE1),
                rounder.RoundLayoutValue(2));

            dc.DrawLine(
                pen,
                new Point(0, 0),
                _isVertical
                    ? new Point(ActualWidth, 0)
                    : new Point(0, ActualHeight));
        }
    }
}