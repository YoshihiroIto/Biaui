using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaFlagBox : FrameworkElement
    {
        #region Background

        public Brush Background
        {
            get => _Background;
            set
            {
                if (value != _Background)
                    SetValue(BackgroundProperty, value);
            }
        }

        private Brush _Background;

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Background = (Brush) e.NewValue;
                    }));

        #endregion

        #region Foreground

        public Brush Foreground
        {
            get => _Foreground;
            set
            {
                if (value != _Foreground)
                    SetValue(ForegroundProperty, value);
            }
        }

        private Brush _Foreground;

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._Foreground = (Brush) e.NewValue;
                    }));

        #endregion

        #region IsPressed

        public bool IsPressed
        {
            get => _IsPressed;
            set
            {
                if (value != _IsPressed)
                    SetValue(IsPressedProperty, Boxes.Bool(value));
            }
        }

        private bool _IsPressed;

        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register(nameof(IsPressed), typeof(bool), typeof(BiaFlagBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaFlagBox) s;
                        self._IsPressed = (bool) e.NewValue;
                    }));

        #endregion

        #region Flag0

        public bool Flag0
        {
            get => _Flag0;
            set
            {
                if (value != _Flag0)
                    SetValue(Flag0Property, value);
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
                    SetValue(Flag1Property, value);
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
                    SetValue(Flag2Property, value);
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
                    SetValue(Flag3Property, value);
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
                    SetValue(Flag4Property, value);
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
                    SetValue(Flag5Property, value);
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
                    SetValue(Flag6Property, value);
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
                    SetValue(Flag7Property, value);
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
            => (_isMouseOver & (1 << index)) != 0;

        private void SetIsMouseOverButton(int index, bool i)
        {
            if (i)
                _isMouseOver |= (1u << index);
            else
                _isMouseOver &= ~(1u << index);

            InvalidateVisual();
        }

        private void ResetIsMouseOverButton()
        {
            _isMouseOver = 0;
            InvalidateVisual();
        }

        private bool IsPressedButton(int index)
            => (_isPressed & (1 << index)) != 0;

        private void SetIsPressedButton(int index, bool i)
        {
            if (i)
                _isPressed |= (1u << index);
            else
                _isPressed &= ~(1u << index);

            InvalidateVisual();
        }

        private void ResetIsPressedButton()
        {
            _isPressed = 0;
            InvalidateVisual();
        }

        private uint _isMouseOver;
        private uint _isPressed;

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

        private void DrawButton(DrawingContext dc, double x, double y, bool isEnabled, bool isChecked, bool isMouseOver,
            bool isPressed)
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
                return _buttonInactiveBackgroundBrushKey;
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsPressed = IsInMouse(e);

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

            if (IsPressed == false)
                return;

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            IsPressed = false;

            UpdateState(e, false);

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
            if (IsPressed)
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
            if (IsPressed)
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