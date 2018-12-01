using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaHsvBox : FrameworkElement
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaHsvBox),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._BorderColor = (Color) e.NewValue;
                    }));

        #endregion

        #region Hue

        public double Hue
        {
            get => _Hue;
            set
            {
                if (NumberHelper.AreClose(value, _Hue) == false)
                    SetValue(HueProperty, value);
            }
        }

        private double _Hue;

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvBox),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._Hue = (double) e.NewValue;
                    }));

        #endregion

        #region Saturation

        public double Saturation
        {
            get => _Saturation;
            set
            {
                if (NumberHelper.AreClose(value, _Saturation) == false)
                    SetValue(SaturationProperty, value);
            }
        }

        private double _Saturation;

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvBox),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._Saturation = (double) e.NewValue;
                    }));

        #endregion

        #region Value

        public double Value
        {
            get => _Value;
            set
            {
                if (NumberHelper.AreClose(value, _Value) == false)
                    SetValue(ValueProperty, value);
            }
        }

        private double _Value = default(double);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaHsvBox),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._Value = (double) e.NewValue;
                    }));

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (value != _IsReadOnly)
                    SetValue(IsReadOnlyProperty, Boxes.Bool(value));
            }
        }

        private bool _IsReadOnly = default(bool);

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var rect = this.RoundLayoutActualRectangle(true);
            var p = this.GetBorderPen(BorderColor);
            var b = GetBackgroundBrush();

            dc.DrawRectangle(b.Hue, null, rect);
            dc.DrawRectangle(b.Saturation, p, rect);

            //
            var c = CursorRenderPos;
            var s = FrameworkElementHelper.RoundLayoutValue(4);
            dc.DrawEllipse(null, Caches.PointOut, c, s, s);
            dc.DrawEllipse(null, IsEnabled == false || IsReadOnly ? Caches.PointInIsReadOnly : Caches.PointIn, c, s, s);
        }

        private const double BorderWidth = 2.0;

        private Point CursorRenderPos
        {
            get
            {
                var bw = (BorderWidth + 2) / WpfHelper.PixelsPerDip;

                var x = Hue * (ActualWidth - bw * 2) + bw;
                var y = (1 - Saturation) * (ActualHeight - bw * 2) + bw;

                return new Point(x, y);
            }
        }

        private void UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var s = FrameworkElementHelper.RoundLayoutValue(1);
            var x = (pos.X - s) / (ActualWidth - s*2);
            var y = (pos.Y - s) / (ActualHeight - s*2);

            x = Math.Min(Math.Max(x, 0), 1);
            y = Math.Min(Math.Max(y, 0), 1);

            Hue = x;
            Saturation = 1 - y;
        }

        private bool _isMouseDown;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsReadOnly)
                return;

            _isMouseDown = true;
            GuiHelper.HideCursor();

            UpdateParams(e);

            // マウス可動域を設定
            {
                var p0 = new Point(0, 0);
                var p1 = new Point(ActualWidth, ActualHeight);
                var dp0 = PointToScreen(p0);
                var dp1 = PointToScreen(p1);

                var cr = new Win32Helper.RECT((int) dp0.X + 1, (int) dp0.Y + 1, (int) dp1.X - 1, (int) dp1.Y - 1);
                Win32Helper.ClipCursor(ref cr);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsReadOnly)
                return;

            if (_isMouseDown == false)
                return;

            UpdateParams(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (IsReadOnly)
                return;

            if (_isMouseDown == false)
                return;

            // マウス位置を補正する
            {
                var p = PointToScreen(CursorRenderPos);
                Win32Helper.SetCursorPos((int) p.X, (int) p.Y);
            }

            _isMouseDown = false;
            GuiHelper.ShowCursor();
            Win32Helper.ClipCursor(IntPtr.Zero);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown)
            {
                _isMouseDown = false;
                GuiHelper.ShowCursor();
                Win32Helper.ClipCursor(IntPtr.Zero);
            }
        }

        private static readonly (Brush Hue, Brush Saturation)[] _brushCache = new (Brush Hue, Brush Saturation)[256];

        private (Brush Hue, Brush Saturation) GetBackgroundBrush()
        {
            var key = (int) (Value * 255);
            key = Math.Min(255, Math.Max(0, key));

            var brush = _brushCache[key];

            if (brush.Hue != null)
                return brush;

            var v = key / 255.0;

            {
                var s0 = new GradientStop(P(0xFF, 0x00, 0x00, v), 0.0 / 6);
                var s1 = new GradientStop(P(0xFF, 0xFF, 0x00, v), 1.0 / 6);
                var s2 = new GradientStop(P(0x00, 0xFF, 0x00, v), 2.0 / 6);
                var s3 = new GradientStop(P(0x00, 0xFF, 0xFF, v), 3.0 / 6);
                var s4 = new GradientStop(P(0x00, 0x00, 0xFF, v), 4.0 / 6);
                var s5 = new GradientStop(P(0xFF, 0x00, 0xFF, v), 5.0 / 6);
                var s6 = new GradientStop(P(0xFF, 0x00, 0x00, v), 6.0 / 6);

                s0.Freeze();
                s1.Freeze();
                s2.Freeze();
                s3.Freeze();
                s4.Freeze();
                s5.Freeze();
                s6.Freeze();

                var c = new GradientStopCollection {s0, s1, s2, s3, s4, s5, s6};
                c.Freeze();

                brush.Hue = new LinearGradientBrush(c, 0);
                brush.Hue.Freeze();
            }

            {
                var s0 = new GradientStop(PA(0x00, 0xFF, 0xFF, 0xFF, v), 0.0);
                var s1 = new GradientStop(P(0xFF, 0xFF, 0xFF, v), 1.0);

                s0.Freeze();
                s1.Freeze();

                var c = new GradientStopCollection {s0, s1,};
                c.Freeze();

                brush.Saturation = new LinearGradientBrush(c, 90);
                brush.Saturation.Freeze();
            }

            _brushCache[key] = brush;

            return brush;

            //////////////////////////////////////////////////////////////////
            Color PA(byte a, byte r, byte g, byte b, double value)
                => Color.FromArgb(a, (byte) (r * value), (byte) (g * value), (byte) (b * value));

            Color P(byte r, byte g, byte b, double value)
                => Color.FromRgb((byte) (r * value), (byte) (g * value), (byte) (b * value));
        }
    }
}