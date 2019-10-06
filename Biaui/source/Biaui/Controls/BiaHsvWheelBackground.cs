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

        private double _Value;

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

        private bool _IsReadOnly;

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

        #region StartedContinuousEditingCommand

        public ICommand StartedContinuousEditingCommand
        {
            get => _StartedContinuousEditingCommand;
            set
            {
                if (value != _StartedContinuousEditingCommand)
                    SetValue(StartedContinuousEditingCommandProperty, value);
            }
        }

        private ICommand _StartedContinuousEditingCommand;

        public static readonly DependencyProperty StartedContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(StartedContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaHsvWheelBackground),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
                        self._StartedContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region EndContinuousEditingCommand

        public ICommand EndContinuousEditingCommand
        {
            get => _EndContinuousEditingCommand;
            set
            {
                if (value != _EndContinuousEditingCommand)
                    SetValue(EndContinuousEditingCommandProperty, value);
            }
        }

        private ICommand _EndContinuousEditingCommand;

        public static readonly DependencyProperty EndContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(EndContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaHsvWheelBackground),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
                        self._EndContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region StartedBatchEditingCommand

        public ICommand StartedBatchEditingCommand
        {
            get => _StartedBatchEditingCommand;
            set
            {
                if (value != _StartedBatchEditingCommand)
                    SetValue(StartedBatchEditingCommandProperty, value);
            }
        }

        private ICommand _StartedBatchEditingCommand;

        public static readonly DependencyProperty StartedBatchEditingCommandProperty =
            DependencyProperty.Register(
                nameof(StartedBatchEditingCommand),
                typeof(ICommand),
                typeof(BiaHsvWheelBackground),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
                        self._StartedBatchEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region EndBatchEditingCommand

        public ICommand EndBatchEditingCommand
        {
            get => _EndBatchEditingCommand;
            set
            {
                if (value != _EndBatchEditingCommand)
                    SetValue(EndBatchEditingCommandProperty, value);
            }
        }

        private ICommand _EndBatchEditingCommand;

        public static readonly DependencyProperty EndBatchEditingCommandProperty =
            DependencyProperty.Register(
                nameof(EndBatchEditingCommand),
                typeof(ICommand),
                typeof(BiaHsvWheelBackground),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelBackground) s;
                        self._EndBatchEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        private readonly HsvWheelBackgroundEffect _effect = new HsvWheelBackgroundEffect();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly PropertyChangeNotifier _isEnabledChangeNotifier;

        static BiaHsvWheelBackground()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHsvWheelBackground),
                new FrameworkPropertyMetadata(typeof(BiaHsvWheelBackground)));
        }

        public BiaHsvWheelBackground()
        {
            Effect = _effect;

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            _effect.BorderColor = (Color) FindResource("BackgroundBackgroundColorKey");
            _effect.DisableColor = (Color) FindResource("InactiveBackgroundColorKey");

            SizeChanged += (_, __) =>
            {
                (_effect.AspectRatioCorrectionX, _effect.AspectRatioCorrectionY) =
                    BiaHsvWheelCursor.MakeAspectRatioCorrection(ActualWidth, ActualHeight);
            };

            _isEnabledChangeNotifier = new PropertyChangeNotifier(this, IsEnabledProperty);
            _isEnabledChangeNotifier.ValueChanged += (_, __) =>
            {
                _effect.IsEnabled = IsEnabled ? 1.0f : 0.0f;
                InvalidateVisual();
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

            var bw = this.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);

            var width = ActualWidth - bw * 2;
            var height = ActualHeight - bw * 2;

            var x = (pos.X - bw) / width;
            var y = (pos.Y - bw) / height;

            var dx = x - 0.5;
            var dy = y - 0.5;

            var (cx, cy) = BiaHsvWheelCursor.MakeAspectRatioCorrection(ActualWidth, ActualHeight);
            dx *= cx;
            dy *= cy;

            var h = (Math.Atan2(-dy, -dx) + Math.PI) / (2.0 * Math.PI);
            var s = Math.Sqrt(dx * dx + dy * dy) * 2;
            var ss = s;

            h = NumberHelper.Clamp01(h);
            s = NumberHelper.Clamp01(s);

            Hue = h;
            Saturation = s;

            return ss > 1;
        }

        private bool _isMouseDown;
        private bool _isContinuousEdited;
        private (double, double) _ContinuousEditingStartValue;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsReadOnly)
                return;

            if (IsOutSide(e.GetPosition(this)))
                return;

            _isMouseDown = true;
            GuiHelper.HideCursor();


            _ContinuousEditingStartValue = (Hue, Saturation);
            _isContinuousEdited = true;
            if (StartedContinuousEditingCommand != null)
                if (StartedContinuousEditingCommand.CanExecute(null))
                    StartedContinuousEditingCommand.Execute(null);


            UpdateParams(e);

            CaptureMouse();

            this.SetMouseClipping();

            e.Handled = true;
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
                var pos = BiaHsvWheelCursor.MakeCursorRenderPos(this, ActualWidth, ActualHeight, Hue, Saturation);

                var mousePos = PointToScreen(pos);
                Win32Helper.SetCursorPos((int) mousePos.X, (int) mousePos.Y);
            }

            e.Handled = true;
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
            this.ResetMouseClipping();
            ReleaseMouseCapture();

            if (_isContinuousEdited)
            {
                if (EndContinuousEditingCommand != null)
                {
                    if (EndContinuousEditingCommand.CanExecute(null))
                    {
                        var changedValue = (Hue, Saturation);
                        (Hue, Saturation) = _ContinuousEditingStartValue;

                        EndContinuousEditingCommand.Execute(null);

                        if (StartedBatchEditingCommand != null &&
                            StartedBatchEditingCommand.CanExecute(null))
                            StartedBatchEditingCommand.Execute(null);

                        (Hue, Saturation) = changedValue;

                        if (EndBatchEditingCommand != null &&
                            EndBatchEditingCommand.CanExecute(null))
                            EndBatchEditingCommand.Execute(null);
                    }
                }

                _isContinuousEdited = false;
            }

            e.Handled = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown)
            {
                _isMouseDown = false;
                ReleaseMouseCapture();
                GuiHelper.ShowCursor();
                this.ResetMouseClipping();
            }

            e.Handled = true;
        }

        private bool IsOutSide(Point pos)
        {
            var bw = this.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);

            var width = ActualWidth - bw * 2;
            var height = ActualHeight - bw * 2;

            var x = (pos.X - bw) / width;
            var y = (pos.Y - bw) / height;

            var dx = x - 0.5;
            var dy = y - 0.5;

            var (cx, cy) = BiaHsvWheelCursor.MakeAspectRatioCorrection(ActualWidth, ActualHeight);
            dx *= cx;
            dy *= cy;

            var s = Math.Sqrt(dx * dx + dy * dy) * 2;

            return s > 1;
        }
    }
}