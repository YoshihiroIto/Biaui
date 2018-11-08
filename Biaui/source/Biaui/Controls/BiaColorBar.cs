using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorBar : FrameworkElement
    {
        #region Color0

        public Color Color0
        {
            get => _Color0;
            set
            {
                if (value != _Color0)
                    SetValue(Color0Property, value);
            }
        }

        private Color _Color0 = Colors.Black;

        public static readonly DependencyProperty Color0Property =
            DependencyProperty.Register(nameof(Color0), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorBlack,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color0 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Color1

        public Color Color1
        {
            get => _Color1;
            set
            {
                if (value != _Color1)
                    SetValue(Color1Property, value);
            }
        }

        private Color _Color1 = Colors.White;

        public static readonly DependencyProperty Color1Property =
            DependencyProperty.Register(nameof(Color1), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorWhite,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color1 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Orientation

        public Orientation Orientation
        {
            get => _Orientation;
            set
            {
                if (value != _Orientation)
                    SetValue(OrientationProperty, value);
            }
        }

        private Orientation _Orientation = Orientation.Vertical;

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.OrientationVertical,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Orientation = (Orientation) e.NewValue;
                    }));

        #endregion

        #region BorderColor

        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private Color _BorderColor = Colors.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._BorderColor = (Color) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        private Brush _backgroundBrush;
        private bool _isRequestUpdateBackgroundBrush = true;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_isRequestUpdateBackgroundBrush)
            {
                _isRequestUpdateBackgroundBrush = false;
                UpdateBackgroundBrush();
            }

            dc.DrawRectangle(_backgroundBrush, Caches.GetBorderPen(BorderColor, 1), ActualRectangle);
        }

        private void UpdateBackgroundBrush()
        {
            _backgroundBrush = new LinearGradientBrush(Color1, Color0, 90);
            _backgroundBrush.Freeze();
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
    }
}