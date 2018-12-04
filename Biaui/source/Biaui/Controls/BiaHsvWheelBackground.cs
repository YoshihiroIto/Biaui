using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Effects;
using Biaui.Internals;

namespace Biaui.Controls
{
    internal class BiaHsvWheelBackground : Canvas
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
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvWheelBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
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
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvWheelBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
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
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaHsvWheelBackground),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvWheelBackground),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        private readonly HsvWheelBackgroundEffect _effect = new HsvWheelBackgroundEffect();

        public BiaHsvWheelBackground()
        {
            Effect = _effect;

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            SizeChanged += (_, __) =>
            {
                (_effect.AspectRatioCorrectionX, _effect.AspectRatioCorrectionY) =
                    BiaHsvWheelCursor.MakeAspectRatioCorrection(ActualWidth, ActualHeight);
            };
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

        /// <returns>マウスがホイール外を指しているか？</returns>
        private bool UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var bw = FrameworkElementHelper.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);

            var width = ActualWidth - bw * 2;
            var height = ActualHeight - bw * 2;

            var x = (pos.X - bw) / width;
            var y = (pos.Y - bw) / height;

            var dx = x - 0.5;
            var dy = y - 0.5;

            var (cx, cy) = BiaHsvWheelCursor.MakeAspectRatioCorrection(ActualWidth, ActualHeight);
            dx = dx * cx;
            dy = dy * cy;

            var h = (Math.Atan2(-dy, -dx) + Math.PI) / (2.0 * Math.PI);
            var s = Math.Sqrt(dx * dx + dy * dy) * 2;
            var ss = s;

            h = Math.Min(Math.Max(h, 0), 1);
            s = Math.Min(Math.Max(s, 0), 1);

            Hue = h;
            Saturation = s;

            return ss > 1;
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

            var isOut = UpdateParams(e);

            // マウス位置を補正する
            if (isOut)
            {
                var pos = BiaHsvWheelCursor.MakeCursorRenderPos(ActualWidth, ActualHeight, Hue, Saturation);

                var mousePos = PointToScreen(pos);
                Win32Helper.SetCursorPos((int) mousePos.X, (int) mousePos.Y);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (IsReadOnly)
                return;

            if (_isMouseDown == false)
                return;

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