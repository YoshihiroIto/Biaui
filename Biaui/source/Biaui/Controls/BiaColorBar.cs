using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorBar : FrameworkElement
    {
        #region Value

        public double Value
        {
            get => _value;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _value)
                    SetValue(ValueProperty, value);
            }
        }

        private double _value = default(double);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaColorBar),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._value = (double) e.NewValue;
                    }));

        #endregion

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
                new FrameworkPropertyMetadata(
                    Boxes.ColorBlack,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color0 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
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
                new FrameworkPropertyMetadata(
                    Boxes.ColorWhite,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color1 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
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
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._BorderColor = (Color) e.NewValue;
                    }));

        #endregion

        #region IsInverseValue

        public bool IsInverseValue
        {
            get => _IsInverseValue;
            set
            {
                if (value != _IsInverseValue)
                    SetValue(IsInverseValueProperty, value);
            }
        }

        private bool _IsInverseValue = default(bool);

        public static readonly DependencyProperty IsInverseValueProperty =
            DependencyProperty.Register(nameof(IsInverseValue), typeof(bool), typeof(BiaColorBar),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._IsInverseValue = (bool) e.NewValue;
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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaColorBar),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        private Brush _backgroundBrush;
        private bool _isRequestUpdateBackgroundBrush = true;

        protected override void OnRender(DrawingContext dc)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            VisualEdgeMode = EdgeMode.Unspecified;

            if (_isRequestUpdateBackgroundBrush)
            {
                _isRequestUpdateBackgroundBrush = false;
                UpdateBackgroundBrush();
            }

            var rect = FrameworkElementHelper.RoundLayoutRect(0.5, 0.5, ActualWidth - 1.5, ActualHeight - 1.5);
            dc.DrawRectangle(_backgroundBrush, this.GetBorderPen(BorderColor), rect);

            var p = FrameworkElementHelper.RoundLayoutValue(1.5);

            var r = FrameworkElementHelper.RoundLayoutRect(rect.X + p, CursorRenderPosY - 3, rect.Width - p*2, 6);

            dc.DrawRectangle(null, Caches.PointOut, r);
            dc.DrawRectangle(null, IsEnabled == false || IsReadOnly ? Caches.PointInIsReadOnly : Caches.PointIn, r);
        }

        private const double BorderWidth = 2.0;

        private double CursorRenderPosY
        {
            get
            {
                var bw = (BorderWidth + 2) / WpfHelper.PixelsPerDip;
                var y = Value * (ActualHeight - bw * 2) + bw;

                if (IsInverseValue)
                    y = ActualHeight - y;

                return y;
            }
        }

        private void UpdateBackgroundBrush()
        {
            _backgroundBrush = new LinearGradientBrush(Color1, Color0, 90);
            _backgroundBrush.Freeze();
        }

        private void UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var s = FrameworkElementHelper.RoundLayoutValue(1);
            var y = (pos.Y - s) / (ActualHeight - s * 2);
            y = Math.Min(Math.Max(y, 0), 1);

            Value = IsInverseValue ? 1 - y : y;
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
                var p0 = new Point(0.25, 0.25);
                var p1 = new Point(ActualWidth - 0.25, ActualHeight - 0.25);
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
                var x = ActualWidth / 2;
                var y = CursorRenderPosY;

                var p = PointToScreen(new Point(x, y));
                Win32Helper.SetCursorPos((int) p.X, (int) p.Y);
            }

            _isMouseDown = false;
            Win32Helper.ClipCursor(IntPtr.Zero);
            GuiHelper.ShowCursor();
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
        }
    }
}