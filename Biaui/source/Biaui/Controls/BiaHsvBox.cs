using System;
using System.Collections.Generic;
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
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
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
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
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
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._Saturation = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Value

        public double Value
        {
            get => _Value;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Value)
                    SetValue(ValueProperty, value);
            }
        }

        private double _Value = default(double);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaHsvBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._Value = (double) e.NewValue;
                        self.InvalidateVisual();
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
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaHsvBox) s;
                        self._IsReadOnly = (bool) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var rect = new Rect(0.5, 0.5, ActualWidth - 1, ActualHeight - 1);

            dc.PushGuidelineSet(Caches.GetGuidelineSet(rect, BorderWidth));
            {
                var p = Caches.GetBorderPen(BorderColor, 2 / WpfHelper.PixelsPerDip);

                var b = GetBackgroundBrush();
                dc.DrawRectangle(b.Hue, null, rect);
                dc.DrawRectangle(b.Saturation, p, rect);

                //
                var c = CursorRenderPos;
                var s = 3.0;
                dc.DrawEllipse(null, Caches.PointOut, c, s, s);
                dc.DrawEllipse(null, IsReadOnly ? Caches.PointInIsReadOnly : Caches.PointIn, c, s, s);
            }
            dc.Pop();
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
                var cr = new Win32Helper.RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
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
                Win32Helper.SetCursorPos((int)p.X, (int)p.Y);
            }

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
                Win32Helper.ClipCursor(IntPtr.Zero);
            }

            InvalidateVisual();
        }

        private static readonly Dictionary<double, (Brush Hue, Brush Saturation)> _brushCache =
            new Dictionary<double, (Brush Hue, Brush Saturation)>();

        private (Brush Hue, Brush Saturation) GetBackgroundBrush()
        {
            if (_brushCache.TryGetValue(Value, out var brush))
                return brush;

            var v = Value;

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

            _brushCache.Add(Value, brush);

            return brush;

            //////////////////////////////////////////////////////////////////
            Color PA(byte a, byte r, byte g, byte b, double value)
                => Color.FromArgb(a, (byte) (r * value), (byte) (g * value), (byte) (b * value));

            Color P(byte r, byte g, byte b, double value)
                => Color.FromRgb((byte) (r * value), (byte) (g * value), (byte) (b * value));
        }
    }
}