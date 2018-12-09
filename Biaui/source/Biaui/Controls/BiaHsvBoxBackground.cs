using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Effects;
using Biaui.Internals;

namespace Biaui.Controls
{
    internal class BiaHsvBoxBackground : Canvas
    {
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
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvBoxBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxBackground) s;
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
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvBoxBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxBackground) s;
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
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaHsvBoxBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxBackground) s;
                        self._Value = (double) e.NewValue;

                        self._effect.Value = self._Value;
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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvBoxBackground),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxBackground) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        private readonly HsvBoxBackgroundEffect _effect = new HsvBoxBackgroundEffect();

        public BiaHsvBoxBackground()
        {
            Effect = _effect;

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var rect = this.RoundLayoutActualRectangle(true);

            dc.DrawRectangle(Brushes.Transparent, null, rect);
        }

        private void UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var s = FrameworkElementHelper.RoundLayoutValue(1);
            var x = (pos.X - s) / (ActualWidth - s * 2);
            var y = (pos.Y - s) / (ActualHeight - s * 2);

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
                var pos = BiaHsvBoxCursor.MakeCursorRenderPos(ActualWidth, ActualHeight, Hue, Saturation);

                var mousePos = PointToScreen(pos);
                Win32Helper.SetCursorPos((int) mousePos.X, (int) mousePos.Y);
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
    }
}