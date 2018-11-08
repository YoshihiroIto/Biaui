using System;
using System.Windows;
using System.Windows.Input;
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

        #region Hue

        public double Hue
        {
            get => _Hue;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Hue)
                    SetValue(HueProperty, value);
            }
        }

        private double _Hue;

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHueSaturationBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHueSaturationBox) s;
                        self._Hue = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Saturation

        public double Saturation
        {
            get => _Saturation;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Saturation)
                    SetValue(SaturationProperty, value);
            }
        }

        private double _Saturation;

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHueSaturationBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHueSaturationBox) s;
                        self._Saturation = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            var p = Caches.GetBorderPen(BorderColor, 1);

            var r = new Rect(0, 0, ActualWidth, ActualHeight);

            dc.DrawRectangle(_hueBrush, p, r);
            dc.DrawRectangle(_valueBrush, p, r);

            var x = Hue * ActualWidth;
            var y = (1 - Saturation) * ActualHeight;

            var c = new Point(x, y);
            var s = 3.0;
            dc.DrawEllipse(null, Caches.PointOut, c, s, s);
            dc.DrawEllipse(null, Caches.PointIn, c, s, s);
        }

        private void UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var x = pos.X / (ActualWidth - borderSize);
            var y = pos.Y / (ActualHeight - borderSize);

            x = Math.Min(Math.Max(x, 0), 1);
            y = Math.Min(Math.Max(y, 0), 1);

            Hue = x;
            Saturation = 1 - y;
        }

        private const double borderSize = 1.0;

        private bool _isMouseDown;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isMouseDown = true;
            GuiHelper.HideCursor();

            UpdateParams(e);

            // マウス可動域を設定
            {
                var p0 = new Point(0, 0);
                var p1 = new Point(ActualWidth, ActualHeight);
                var dp0 = PointToScreen(p0);
                var dp1 = PointToScreen(p1);
                var cr = new Win32Helper.RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
                Win32Helper.ClipCursor(ref cr);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown == false)
                return;

            UpdateParams(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _isMouseDown = false;
            GuiHelper.ShowCursor();
            Win32Helper.ClipCursor(IntPtr.Zero);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown)
            {
                _isMouseDown = false;
                ReleaseMouseCapture();
                Win32Helper.ClipCursor(IntPtr.Zero);
            }

            InvalidateVisual();
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