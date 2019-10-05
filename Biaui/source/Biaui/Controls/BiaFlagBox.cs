using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaFlagBox : FrameworkElement
    {
        #region Flag0

        public bool Flag0
        {
            get => _Flag0;
            set
            {
                if (value != _Flag0)
                    SetValue(Flag0Property, Boxes.Bool(value));
            }
        }

        private bool _Flag0;

        public static readonly DependencyProperty Flag0Property =
            DependencyProperty.Register(
                nameof(Flag0),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag0 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag1

        public bool Flag1
        {
            get => _Flag1;
            set
            {
                if (value != _Flag1)
                    SetValue(Flag1Property, Boxes.Bool(value));
            }
        }

        private bool _Flag1;

        public static readonly DependencyProperty Flag1Property =
            DependencyProperty.Register(
                nameof(Flag1),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag1 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag2

        public bool Flag2
        {
            get => _Flag2;
            set
            {
                if (value != _Flag2)
                    SetValue(Flag2Property, Boxes.Bool(value));
            }
        }

        private bool _Flag2;

        public static readonly DependencyProperty Flag2Property =
            DependencyProperty.Register(
                nameof(Flag2),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag2 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag3

        public bool Flag3
        {
            get => _Flag3;
            set
            {
                if (value != _Flag3)
                    SetValue(Flag3Property, Boxes.Bool(value));
            }
        }

        private bool _Flag3;

        public static readonly DependencyProperty Flag3Property =
            DependencyProperty.Register(
                nameof(Flag3),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag3 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag4

        public bool Flag4
        {
            get => _Flag4;
            set
            {
                if (value != _Flag4)
                    SetValue(Flag4Property, Boxes.Bool(value));
            }
        }

        private bool _Flag4;

        public static readonly DependencyProperty Flag4Property =
            DependencyProperty.Register(
                nameof(Flag4),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag4 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag5

        public bool Flag5
        {
            get => _Flag5;
            set
            {
                if (value != _Flag5)
                    SetValue(Flag5Property, Boxes.Bool(value));
            }
        }

        private bool _Flag5;

        public static readonly DependencyProperty Flag5Property =
            DependencyProperty.Register(
                nameof(Flag5),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag5 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag6

        public bool Flag6
        {
            get => _Flag6;
            set
            {
                if (value != _Flag6)
                    SetValue(Flag6Property, Boxes.Bool(value));
            }
        }

        private bool _Flag6;

        public static readonly DependencyProperty Flag6Property =
            DependencyProperty.Register(
                nameof(Flag6),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag6 = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag7

        public bool Flag7
        {
            get => _Flag7;
            set
            {
                if (value != _Flag7)
                    SetValue(Flag7Property, Boxes.Bool(value));
            }
        }

        private bool _Flag7;

        public static readonly DependencyProperty Flag7Property =
            DependencyProperty.Register(
                nameof(Flag7),
                typeof(bool),
                typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Flag7 = (bool) e.NewValue;
                    }));

        #endregion

        private uint AllFlags
        {
            get
            {
                uint f = 0;

                if (Flag0) f |= 1 << 0;
                if (Flag1) f |= 1 << 1;
                if (Flag2) f |= 1 << 2;
                if (Flag3) f |= 1 << 3;
                if (Flag4) f |= 1 << 4;
                if (Flag5) f |= 1 << 5;
                if (Flag6) f |= 1 << 6;
                if (Flag7) f |= 1 << 7;

                return f;
            }

            set
            {
                Flag0 = (value & (1 << 0)) != 0;
                Flag1 = (value & (1 << 1)) != 0;
                Flag2 = (value & (1 << 2)) != 0;
                Flag3 = (value & (1 << 3)) != 0;
                Flag4 = (value & (1 << 4)) != 0;
                Flag5 = (value & (1 << 5)) != 0;
                Flag6 = (value & (1 << 6)) != 0;
                Flag7 = (value & (1 << 7)) != 0;
            }
        }

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
                typeof(BiaFlagBox),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
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
                typeof(BiaFlagBox),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
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
                typeof(BiaFlagBox),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
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
                typeof(BiaFlagBox),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._EndBatchEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        private bool _isPressed;

        private bool IsCheckedButton(int index)
        {
            switch (index)
            {
                case 0: return Flag0;
                case 1: return Flag1;
                case 2: return Flag2;
                case 3: return Flag3;
                case 4: return Flag4;
                case 5: return Flag5;
                case 6: return Flag6;
                case 7: return Flag7;
            }

            throw new NotImplementedException();
        }

        private void SetIsCheckedButton(int index, bool i)
        {
            switch (index)
            {
                case 0:
                    Flag0 = i;
                    return;
                case 1:
                    Flag1 = i;
                    return;
                case 2:
                    Flag2 = i;
                    return;
                case 3:
                    Flag3 = i;
                    return;
                case 4:
                    Flag4 = i;
                    return;
                case 5:
                    Flag5 = i;
                    return;
                case 6:
                    Flag6 = i;
                    return;
                case 7:
                    Flag7 = i;
                    return;
            }

            throw new NotImplementedException();
        }

        private bool IsMouseOverButton(int index)
            => (_isMouseOverButton & (1 << index)) != 0;

        private void SetIsMouseOverButton(int index, bool i)
        {
            if (i)
                _isMouseOverButton |= (1u << index);
            else
                _isMouseOverButton &= ~(1u << index);

            InvalidateVisual();
        }

        private void ResetIsMouseOverButton()
        {
            _isMouseOverButton = 0;
            InvalidateVisual();
        }

        private bool IsPressedButton(int index)
            => (_isPressedButton & (1 << index)) != 0;

        private void SetIsPressedButton(int index, bool i)
        {
            if (i)
                _isPressedButton |= (1u << index);
            else
                _isPressedButton &= ~(1u << index);

            InvalidateVisual();
        }

        private void ResetIsPressedButton()
        {
            _isPressedButton = 0;
            InvalidateVisual();
        }

        private uint _isMouseOverButton;
        private uint _isPressedButton;

        static BiaFlagBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(typeof(BiaFlagBox)));
        }

        public BiaFlagBox()
        {
            Width = ColumnCount * ButtonWidth;
            Height = RowCount * ButtonHeight;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            // 背景
            {
                dc.PushClip(Caches.GetClipGeom(ActualWidth, ActualHeight, Constants.BasicCornerRadiusPrim, true));

                var index = 0;
                var isEnabled = IsEnabled;

                for (var row = 0; row != RowCount; ++row)
                {
                    for (var column = 0; column != ColumnCount; ++column, ++index)
                    {
                        var x = column * ButtonWidth;
                        var y = row * ButtonHeight;

                        var isChecked = IsCheckedButton(index);
                        var isMouseOver = IsMouseOverButton(index);
                        var isPressed = IsPressedButton(index);

                        DrawButton(dc, x, y, isEnabled, isChecked, isMouseOver, isPressed);
                    }
                }

                dc.Pop();
            }

            var borderPen = Caches.GetPen(Color.FromRgb(0x3F, 0x3F, 0x47), FrameworkElementHelper.RoundLayoutValue(1));

            dc.DrawRoundedRectangle(
                null,
                borderPen,
                this.RoundLayoutActualRectangle(false),
                Constants.BasicCornerRadiusPrim,
                Constants.BasicCornerRadiusPrim);


            dc.DrawLine(borderPen, new Point(0, Height * 0.5), new Point(Width, Height * 0.5));

            for (var column = 1; column != ColumnCount; ++column)
            {
                var x = column * ButtonWidth;
                dc.DrawLine(borderPen, new Point(x, 0), new Point(x, Height));
            }
        }

        private void DrawButton(DrawingContext dc, double x, double y, bool isEnabled, bool isChecked, bool isMouseOver, bool isPressed)
        {
            dc.DrawRectangle(
                SelectBrush(isEnabled, isChecked, isMouseOver, isPressed),
                null,
                new Rect(x, y, ButtonWidth, ButtonHeight));
        }

        private static Brush SelectBrush(bool isEnabled, bool isChecked, bool isMouseOver, bool isPressed)
        {
            if (isEnabled == false)
            {
                return isChecked
                    ? _toggleButtonBackgroundBrushKeyIsCheckedIsDisabled
                    : _buttonInactiveBackgroundBrushKey;
            }

            if (isPressed)
            {
                return isChecked
                    ? _toggleButtonBackgroundBrushKeyIsCheckedIsMouseOver
                    : _buttonPressedBackgroundBrushKey;
            }

            if (isMouseOver)
            {
                return isChecked
                    ? _toggleButtonBackgroundBrushKeyIsCheckedIsMouseOver
                    : _buttonActiveBackgroundBrushKey;
            }

            return isChecked
                ? _toggleButtonBackgroundBrushKeyIsChecked
                : _buttonBackgroundBrushKey;
        }

        #region Brush

        private static readonly Brush _buttonInactiveBackgroundBrushKey =
            (Brush) Application.Current.FindResource("ButtonInactiveBackgroundBrushKey");

        private static readonly Brush _toggleButtonBackgroundBrushKeyIsCheckedIsDisabled =
            (Brush) Application.Current.FindResource("ToggleButtonBackgroundBrushKey.IsChecked.IsDisabled");

        private static readonly Brush _toggleButtonBackgroundBrushKeyIsCheckedIsMouseOver =
            (Brush) Application.Current.FindResource("ToggleButtonBackgroundBrushKey.IsChecked.IsMouseOver");

        private static readonly Brush _toggleButtonBackgroundBrushKeyIsChecked =
            (Brush) Application.Current.FindResource("ToggleButtonBackgroundBrushKey.IsChecked");

        private static readonly Brush _buttonPressedBackgroundBrushKey =
            (Brush) Application.Current.FindResource("ButtonPressedBackgroundBrushKey");

        private static readonly Brush _buttonActiveBackgroundBrushKey =
            (Brush) Application.Current.FindResource("ButtonActiveBackgroundBrushKey");

        private static readonly Brush _buttonBackgroundBrushKey =
            (Brush) Application.Current.FindResource("ButtonBackgroundBrushKey");

        #endregion

        private bool _isContinuousEdited;
        private uint _ContinuousEditingStartValue;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isPressed = IsInMouse(e);

            _ContinuousEditingStartValue = AllFlags;
            _isContinuousEdited = true;
            if (StartedContinuousEditingCommand != null)
                if (StartedContinuousEditingCommand.CanExecute(null))
                    StartedContinuousEditingCommand.Execute(null);

            CaptureMouse();

            UpdateState(e, true);

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            UpdateState(e, false);

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_isPressed == false)
                return;

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            _isPressed = false;

            UpdateState(e, false);

            if (_isContinuousEdited)
            {
                if (EndContinuousEditingCommand != null)
                {
                    if (EndContinuousEditingCommand.CanExecute(null))
                    {
                        var changedValue = AllFlags;
                        AllFlags = _ContinuousEditingStartValue;
                        EndContinuousEditingCommand.Execute(null);

                        StartedBatchEditingCommand?.Execute(null);
                        AllFlags = changedValue;
                        EndBatchEditingCommand?.Execute(null);
                    }
                }

                _isContinuousEdited = false;
            }

            if (IsInMouse(e) == false)
                return;

            e.Handled = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            ResetIsMouseOverButton();
        }

        private bool _continueIsSelected;

        private void UpdateState(MouseEventArgs e, bool isMouseDown)
        {
            var mousePos = e.GetPosition(this);

            var x = (int) (mousePos.X / ButtonWidth);
            var y = (int) (mousePos.Y / ButtonHeight);

            ResetIsMouseOverButton();
            ResetIsPressedButton();

            if (x < 0 || x >= ColumnCount ||
                y < 0 || y >= RowCount)
                return;

            var index = y * ColumnCount + x;

            // IsChecked を作る
            if (_isPressed)
            {
                if (KeyboardHelper.IsPressShift)
                {
                    if (isMouseDown)
                        _continueIsSelected = !IsCheckedButton(index);

                    SetIsCheckedButton(index, _continueIsSelected);
                }
                else
                {
                    for (var i = 0; i != ButtonCount; ++i)
                        SetIsCheckedButton(i, i == index);
                }
            }

            // IsMouseOver を作る
            SetIsMouseOverButton(index, true);

            // IsPressed を作る
            if (_isPressed)
                SetIsPressedButton(index, true);
        }

        private bool IsInMouse(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            return
                pos.X >= 0.0 &&
                pos.X <= ActualWidth &&
                pos.Y >= 0.0 &&
                pos.Y <= ActualHeight;
        }

        private const double ButtonWidth = 16.0;
        private const double ButtonHeight = 16.0;

        private const int ColumnCount = 4;
        private const int RowCount = 2;

        private const int ButtonCount = ColumnCount * RowCount;
    }
}