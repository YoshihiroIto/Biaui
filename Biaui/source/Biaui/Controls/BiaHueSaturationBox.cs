using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaHueSaturationBox : FrameworkElement
    {
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaHueSaturationBox),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaHueSaturationBox) s;
                        self._BorderColor = (Color) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            var p = Caches.GetBorderPen(BorderColor, 1);

            var r = new Rect(0, 0, ActualWidth, ActualHeight);

            dc.DrawRectangle(_hueBrush, p, r);
            dc.DrawRectangle(_valueBrush, p, r);
        }

        private static readonly Brush _hueBrush;
        private static readonly Brush _valueBrush;

        static BiaHueSaturationBox()
        {
            {
                var s0 = new GradientStop(Color.FromRgb(0xFF, 0x00, 0x00), 0.0 / 6);
                var s1 = new GradientStop(Color.FromRgb(0xFF, 0xFF, 0x00), 1.0 / 6);
                var s2 = new GradientStop(Color.FromRgb(0x00, 0xFF, 0x00), 2.0 / 6);
                var s3 = new GradientStop(Color.FromRgb(0x00, 0xFF, 0xFF), 3.0 / 6);
                var s4 = new GradientStop(Color.FromRgb(0x00, 0x00, 0xFF), 4.0 / 6);
                var s5 = new GradientStop(Color.FromRgb(0xFF, 0x00, 0xFF), 5.0 / 6);
                var s6 = new GradientStop(Color.FromRgb(0xFF, 0x00, 0x00), 6.0 / 6);

                s0.Freeze();
                s1.Freeze();
                s2.Freeze();
                s3.Freeze();
                s4.Freeze();
                s5.Freeze();
                s6.Freeze();

                var c = new GradientStopCollection
                {
                    s0, s1, s2, s3, s4, s5, s6
                };
                c.Freeze();

                _hueBrush = new LinearGradientBrush(c, 0);
                _hueBrush.Freeze();
            }

            {
                var s0 = new GradientStop(Color.FromArgb(0x00, 0x00, 0x00, 0x00), 0.0);
                var s1 = new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 1.0);

                s0.Freeze();
                s1.Freeze();

                var c = new GradientStopCollection
                {
                    s0, s1,
                };
                c.Freeze();

                _valueBrush = new LinearGradientBrush(c, 90);
                _valueBrush.Freeze();
            }
        }
    }
}