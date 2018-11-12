using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls
{
    public enum BiaNumberEditorMode
    {
        Simple,
        WideRange
    }

    public class BiaNumberEditor : FrameworkElement
    {
        #region IsReadOnly

        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (value != _IsReadOnly)
                    SetValue(IsReadOnlyProperty, value);
            }
        }

        private bool _IsReadOnly = default(bool);

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._IsReadOnly = (bool) e.NewValue;
                        self.InvalidateVisual();
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._BorderColor = (Color) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region SliderBrush

        public Brush SliderBrush
        {
            get => _SliderBrush;
            set
            {
                if (value != _SliderBrush)
                    SetValue(SliderBrushProperty, value);
            }
        }

        private Brush _SliderBrush;

        public static readonly DependencyProperty SliderBrushProperty =
            DependencyProperty.Register(nameof(SliderBrush), typeof(Brush), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Brushes.GreenYellow,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._SliderBrush = (Brush) e.NewValue;
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
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        // 最小値・最大値でクランプして保存する

                        var self = (BiaNumberEditor) s;
                        self._Value = self.ClampValue((double) e.NewValue);
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Caption

        public string Caption
        {
            get => _Caption;
            set
            {
                if (value != _Caption)
                    SetValue(CaptionProperty, value);
            }
        }

        private string _Caption = default(string);

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(string), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    default(string),
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Caption = (string) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region SliderMinimum

        public double SliderMinimum
        {
            get => _SliderMinimum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _SliderMinimum)
                    SetValue(SliderMinimumProperty, value);
            }
        }

        private double _SliderMinimum = default(double);

        public static readonly DependencyProperty SliderMinimumProperty =
            DependencyProperty.Register(nameof(SliderMinimum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._SliderMinimum = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region SliderMaximum

        public double SliderMaximum
        {
            get => _SliderMaximum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _SliderMaximum)
                    SetValue(SliderMaximumProperty, value);
            }
        }

        private double _SliderMaximum = 100.0;

        public static readonly DependencyProperty SliderMaximumProperty =
            DependencyProperty.Register(nameof(SliderMaximum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double100,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._SliderMaximum = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Minimum

        public double Minimum
        {
            get => _Minimum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Minimum)
                    SetValue(MinimumProperty, value);
            }
        }

        private double _Minimum = default(double);

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        // 変更後の最小値でValueをクランプする

                        var self = (BiaNumberEditor) s;
                        self._Minimum = (double) e.NewValue;

                        self.Value = self.ClampValue(self.Value);

                        self.InvalidateVisual();
                    }));

        #endregion

        #region Maximum

        public double Maximum
        {
            get => _Maximum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Maximum)
                    SetValue(MaximumProperty, value);
            }
        }

        private double _Maximum = 100.0;

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double100,
                    (s, e) =>
                    {
                        // 変更後の最大値でValueをクランプする

                        var self = (BiaNumberEditor) s;
                        self._Maximum = (double) e.NewValue;

                        self.Value = self.ClampValue(self.Value);

                        self.InvalidateVisual();
                    }));

        #endregion

        #region DisplayFormat

        public string DisplayFormat
        {
            get => _DisplayFormat;
            set
            {
                if (value != _DisplayFormat)
                    SetValue(DisplayFormatProperty, value);
            }
        }

        private string _DisplayFormat = "F3";

        public static readonly DependencyProperty DisplayFormatProperty =
            DependencyProperty.Register(nameof(DisplayFormat), typeof(string), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    "F3",
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._DisplayFormat = (string) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region UnitString

        public string UnitString
        {
            get => _UnitString;
            set
            {
                if (value != _UnitString)
                    SetValue(UnitStringProperty, value);
            }
        }

        private string _UnitString = "";

        public static readonly DependencyProperty UnitStringProperty =
            DependencyProperty.Register(nameof(UnitString), typeof(string), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    "",
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._UnitString = (string) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

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

        private Brush _Background = default(Brush);

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Background = (Brush) e.NewValue;
                        self.InvalidateVisual();
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

        private Brush _Foreground = default(Brush);

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Foreground = (Brush) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Padding

        public Thickness Padding
        {
            get => _Padding;
            set
            {
                if (value != _Padding)
                    SetValue(PaddingProperty, value);
            }
        }

        private Thickness _Padding = default(Thickness);

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Thickness0,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Padding = (Thickness) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Mode

        public BiaNumberEditorMode Mode
        {
            get => _Mode;
            set
            {
                if (value != _Mode)
                    SetValue(ModeProperty, value);
            }
        }

        private BiaNumberEditorMode _Mode = BiaNumberEditorMode.Simple;

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(BiaNumberEditorMode), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.BiaNumberModeSimple,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Mode = (BiaNumberEditorMode) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Increment

        public double Increment
        {
            get => _Increment;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Increment)
                    SetValue(IncrementProperty, value);
            }
        }

        private double _Increment = 1.0;

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Increment = (double) e.NewValue;
                    }));

        #endregion

        #region CornerRadius

        public double CornerRadius
        {
            get => _CornerRadius;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _CornerRadius)
                    SetValue(CornerRadiusProperty, value);
            }
        }

        private double _CornerRadius = Constants.BasicCornerRadiusPrim;

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.ConstantsBasicCornerRadiusPrim,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._CornerRadius = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region IsVisibleBorder
        
        public bool IsVisibleBorder
        {
            get => _IsVisibleBorder;
            set
            {
                if (value != _IsVisibleBorder)
                    SetValue(IsVisibleBorderProperty, value);
            }
        }
        
        private bool _IsVisibleBorder = true;
        
        public static readonly DependencyProperty IsVisibleBorderProperty =
            DependencyProperty.Register(nameof(IsVisibleBorder), typeof(bool), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._IsVisibleBorder = (bool)e.NewValue;
                    }));
        
        #endregion

        static BiaNumberEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNumberEditor),
                new FrameworkPropertyMetadata(typeof(BiaNumberEditor)));

            SetupSpinGeom();
        }

        public BiaNumberEditor()
        {
            IsEnabledChanged += (_, __) => InvalidateVisual();
        }

        private const double BorderWidth = 2.0;

        protected override void OnRender(DrawingContext dc)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator

            var rect = new Rect(0.5, 0.5, ActualWidth - 1, ActualHeight - 1);
            dc.PushGuidelineSet(Caches.GetGuidelineSet(rect, BorderWidth));

            if (CornerRadius != 0)
                dc.PushClip(ClipGeom);
            {
                DrawBackground(dc);

                if (Mode == BiaNumberEditorMode.Simple)
                    DrawSlider(dc);

                DrawText(dc);

                if (IsReadOnly == false && IsEnabled && Increment != 0)
                    DrawSpin(dc);

                if (IsVisibleBorder)
                    DrawBorder(dc);
            }
            if (CornerRadius != 0)
                dc.Pop();

            dc.Pop();

            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        private void DrawBackground(DrawingContext dc)
        {
            var brush = _isInPopup ? _textBox.Background : Background;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (CornerRadius == 0)
                dc.DrawRectangle(
                    brush,
                    null,
                    ActualRectangle, null);
            else
                dc.DrawRoundedRectangle(
                    brush,
                    null,
                    ActualRectangle, null,
                    CornerRadius, null,
                    CornerRadius, null);
        }

        private void DrawBorder(DrawingContext dc)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (CornerRadius == 0)
                dc.DrawRectangle(
                    null,
                    Caches.GetBorderPen(BorderColor, BorderWidth),
                    ActualRectangle, null
                );
            else
                dc.DrawRoundedRectangle(
                    null,
                    Caches.GetBorderPen(BorderColor, BorderWidth),
                    ActualRectangle, null,
                    CornerRadius, null,
                    CornerRadius, null);
        }

        private void DrawSlider(DrawingContext dc)
        {
            if (SliderWidth <= 0.0f)
                return;

            var w = (UiValue - ActualSliderMinimum) * ActualWidth / SliderWidth;
            var brush = _isInPopup ? _textBox.Background : SliderBrush;

            dc.DrawRectangle(brush, null, new Rect(0, 0, w, ActualHeight));
        }

        private const double SpinWidth = 14.0;

        private void DrawText(DrawingContext dc)
        {
            TextRenderer.Default.Draw(
                Caption,
                Padding.Left + SpinWidth,
                Padding.Top,
                Foreground,
                dc,
                ActualWidth - Padding.Left - Padding.Right,
                TextAlignment.Left
            );

            TextRenderer.Default.Draw(
                UiValueString,
                Padding.Left,
                Padding.Top,
                Foreground,
                dc,
                ActualWidth - Padding.Left - Padding.Right - SpinWidth,
                TextAlignment.Right
            );
        }

        private void DrawSpin(DrawingContext dc)
        {
            var moBrush = Application.Current.FindResource("AccentBrushKey") as Brush;

            {
                var offsetX = 5.0;
                var offsetY = 8.0;

                if (_mouseOverType == MouseOverType.DecSpin)
                    dc.DrawRectangle(_SpinBackground, null, new Rect(0, 0, SpinWidth, ActualHeight));

                var key = (offsetX, offsetY);
                if (_TranslateTransformCache.TryGetValue(key, out var tt) == false)
                {
                    tt = new TranslateTransform(offsetX, offsetY);
                    _TranslateTransformCache.Add(key, tt);
                }

                dc.PushTransform(tt);
                dc.DrawGeometry(
                    _mouseOverType == MouseOverType.DecSpin ? moBrush : Foreground, null, _DecSpinGeom);
                dc.Pop();
            }

            {
                var offsetX = ActualWidth - 5.0 * 2 + 1;
                var offsetY = 8.0;

                if (_mouseOverType == MouseOverType.IncSpin)
                    dc.DrawRectangle(_SpinBackground, null,
                        new Rect(ActualWidth - SpinWidth, 0, SpinWidth, ActualHeight));

                var key = (offsetX, offsetY);
                if (_TranslateTransformCache.TryGetValue(key, out var tt) == false)
                {
                    tt = new TranslateTransform(offsetX, offsetY);
                    _TranslateTransformCache.Add(key, tt);
                }

                dc.PushTransform(tt);
                dc.DrawGeometry(
                    _mouseOverType == MouseOverType.IncSpin ? moBrush : Foreground, null, _IncSpinGeom);
                dc.Pop();
            }
        }

        private enum MouseOverType
        {
            None,
            DecSpin,
            IncSpin,
            Slider
        }

        private bool _isMouseDown;
        private bool _isMouseMoved;
        private Point _oldPos;
        private Point _mouseDownPos;
        private MouseOverType _mouseOverTypeOnMouseDown;
        private MouseOverType _mouseOverType;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsReadOnly)
                return;

            _isMouseDown = true;
            _isMouseMoved = false;
            _oldPos = e.GetPosition(this);
            _mouseDownPos = _oldPos;
            _mouseOverTypeOnMouseDown = MakeMouseOverType(e);

            if (_mouseOverTypeOnMouseDown == MouseOverType.Slider)
            {
                CaptureMouse();

                // マウス可動域を設定
                if (Mode == BiaNumberEditorMode.Simple)
                {
                    var p0 = new Point(0.0, 0.0);
                    var p1 = new Point(ActualWidth + 1, ActualHeight + 1);
                    var dp0 = PointToScreen(p0);
                    var dp1 = PointToScreen(p1);
                    var cr = new Win32Helper.RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
                    Win32Helper.ClipCursor(ref cr);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _mouseOverType = MakeMouseOverType(e);
            InvalidateVisual();

            if (_isMouseDown == false)
                return;

            if (IsReadOnly)
                return;

            // Down直後マウス移動ない場合でもOnMouseMoveが呼ばれることがあるため、判定する。
            var currentPos = e.GetPosition(this);
            if (currentPos == _oldPos)
                return;

            if (_mouseOverTypeOnMouseDown != MouseOverType.Slider)
                return;

            switch (Mode)
            {
                case BiaNumberEditorMode.Simple:
                {
                    // 0から1
                    var xr = Math.Min(Math.Max(0, currentPos.X), ActualWidth) / ActualWidth;
                    Value = SliderWidth * xr + ActualSliderMinimum;
                    break;
                }

                case BiaNumberEditorMode.WideRange:
                {
                    if (_isMouseMoved == false)
                        GuiHelper.HideCursor();

                    // Ctrl押下中は５倍速い
                    var s = IsCtrl ? 5.0 : 1.0;
                    var w = currentPos.X - _oldPos.X;
                    var v = Value + s * w * Increment;

                    Value = Math.Min(ActualSliderMaximum, Math.Max(ActualSliderMinimum, v));

                    // 移動量だけ取れれば良いので、現在位置をスタート位置に戻す
                    var p = PointToScreen(_mouseDownPos);
                    Win32Helper.SetCursorPos((int) p.X, (int) p.Y);
                    currentPos = _mouseDownPos;

                    break;
                }
            }

            _mouseOverType = MouseOverType.Slider;
            _isMouseMoved = true;
            _oldPos = currentPos;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsReadOnly == false)
            {
                if (_mouseOverTypeOnMouseDown == MouseOverType.Slider)
                {
                    ReleaseMouseCapture();

                    switch (Mode)
                    {
                        case BiaNumberEditorMode.Simple:
                            Win32Helper.ClipCursor(IntPtr.Zero);
                            break;

                        case BiaNumberEditorMode.WideRange:
                        {
                            var p = PointToScreen(_mouseDownPos);
                            Win32Helper.SetCursorPos((int) p.X, (int) p.Y);
                            GuiHelper.ShowCursor();
                            break;
                        }
                    }
                }
            }

            _isMouseDown = false;

            if (_isMouseMoved == false)
            {
                var p = e.GetPosition(this);

                if (p == _mouseDownPos)
                {
                    // Ctrl押下中は５倍速い
                    var inc = IsCtrl ? Increment * 5 : Increment;

                    if (p.X <= SpinWidth && IsReadOnly == false)
                        AddValue(-inc);
                    else if (p.X >= ActualWidth - SpinWidth && IsReadOnly == false)
                        AddValue(inc);
                    else
                        ShowEditBox();
                }
            }

            _mouseOverType = MakeMouseOverType(e);

            InvalidateVisual();
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
                GuiHelper.ShowCursor();
            }

            _mouseOverType = MouseOverType.None;

            InvalidateVisual();
        }

        private MouseOverType MakeMouseOverType(MouseEventArgs e)
        {
            var x = e.GetPosition(this).X;

            if (x < 0)
                return MouseOverType.None;

            if (x >= ActualWidth)
                return MouseOverType.None;

            if (x <= SpinWidth)
                return MouseOverType.DecSpin;

            if (x >= ActualWidth - SpinWidth)
                return MouseOverType.IncSpin;

            return MouseOverType.Slider;
        }

        private static readonly TextBox _textBox = new TextBox
        {
            IsTabStop = false,
            IsUndoEnabled = false
        };

        private static readonly Popup _popup = new Popup
        {
            Child = _textBox,
            AllowsTransparency = true,
            StaysOpen = false,
        };

        private bool _isInPopup;
        private PopupResult _popupResult;

        private enum PopupResult
        {
            Ok,
            Cancel
        }

        private void AddValue(double i)
        {
            var v = Value + i;

            Value = Math.Min(ActualMaximum, Math.Max(ActualMinimum, v));
        }

        private void ShowEditBox()
        {
            _textBox.Width = ActualWidth;
            _textBox.Height = ActualHeight;
            _textBox.IsReadOnly = IsReadOnly;
            _textBox.Text = FormattedValueString;
            _textBox.TextChanged += TextBoxOnTextChanged;

            _popup.PlacementTarget = this;
            _popup.VerticalOffset = -ActualHeight;
            _popup.Closed += PopupOnClosed;
            _popup.PreviewKeyDown += PopupOnPreviewKeyDown;

            _popupResult = PopupResult.Ok;
            _popup.IsOpen = true;
            _isInPopup = true;
            InvalidateVisual();

            _textBox.Focus();
            _textBox.SelectAll();
        }

        private void PopupOnClosed(object sender, EventArgs e)
        {
            if (_popupResult == PopupResult.Ok)
                ConfirmValue();

            _popup.Closed -= PopupOnClosed;
            _popup.PreviewKeyDown -= PopupOnPreviewKeyDown;
            _textBox.TextChanged -= TextBoxOnTextChanged;

            _isInPopup = false;
            InvalidateVisual();

            void ConfirmValue()
            {
                var v = MakeValueFromString(_textBox.Text);
                if (v.Result == MakeValueResult.Ok ||
                    v.Result == MakeValueResult.Continue)
                    Value = v.Value;
            }
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void PopupOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                case Key.Tab:
                    var v = MakeValueFromString(_textBox.Text);
                    if (v.Result == MakeValueResult.Continue)
                        _textBox.Text = v.Value.ToString(DisplayFormat);
                    else
                        Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) ClosePopup);
                    break;

                case Key.Escape:
                    _popupResult = PopupResult.Cancel;
                    _textBox.Text = FormattedValueString;
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) ClosePopup);
                    break;
            }

            void ClosePopup() => ((Popup) sender).IsOpen = false;
        }

        private enum MakeValueResult
        {
            Ok,
            Cancel,
            Continue
        }

        private static Regex _evalRegex;

        private (MakeValueResult Result, double Value) MakeValueFromString(string src)
        {
            if (double.TryParse(src, out var v))
                return (MakeValueResult.Ok, Math.Min(ActualMaximum, Math.Max(ActualMinimum, v)));

            if (double.TryParse(Evaluator.Eval(src), out v))
                return (MakeValueResult.Continue, Math.Min(ActualMaximum, Math.Max(ActualMinimum, v)));

            // Math.を補間
            if (_evalRegex == null)
                _evalRegex = new Regex("[A-Za-z_]+");

            var rs = _evalRegex.Replace(src, "Math.$0");
            if (double.TryParse(Evaluator.Eval(rs), out v))
                return (MakeValueResult.Continue, Math.Min(ActualMaximum, Math.Max(ActualMinimum, v)));

            return (MakeValueResult.Cancel, default(double));
        }

        private static void SetupSpinGeom()
        {
            {
                var start = new Point(0, 4);

                var segments = new[]
                {
                    new LineSegment(new Point(4, 0), true),
                    new LineSegment(new Point(4, 8), true)
                };

                segments[0].Freeze();
                segments[1].Freeze();

                var figure = new PathFigure(start, segments, true);
                figure.Freeze();

                _DecSpinGeom = new PathGeometry(new[] {figure});
                _DecSpinGeom.Freeze();
            }

            {
                var start = new Point(4, 4);

                var segments = new[]
                {
                    new LineSegment(new Point(0, 0), true),
                    new LineSegment(new Point(0, 8), true)
                };

                segments[0].Freeze();
                segments[1].Freeze();

                var figure = new PathFigure(start, segments, true);
                figure.Freeze();

                _IncSpinGeom = new PathGeometry(new[] {figure});
                _IncSpinGeom.Freeze();
            }

            _SpinBackground = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x00, 0x00));
        }

        private double ClampValue(double v)
            => Math.Max(Math.Min(v, ActualMaximum), ActualMinimum);

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
        private string FormattedValueString => Value.ToString(DisplayFormat);

        private double SliderWidth => Math.Abs(SliderMaximum - SliderMinimum);

        private double ActualSliderMinimum => Math.Min(SliderMinimum, SliderMaximum);
        private double ActualSliderMaximum => Math.Max(SliderMinimum, SliderMaximum);
        private double ActualMinimum => Math.Min(Minimum, Maximum);
        private double ActualMaximum => Math.Max(Minimum, Maximum);

        private string UiValueString
        {
            get
            {
                if (_isInPopup == false)
                    return Concat(FormattedValueString, UnitString);

                var v = MakeValueFromString(_textBox.Text);

                return
                    v.Result == MakeValueResult.Ok || v.Result == MakeValueResult.Continue
                        ? Concat(v.Value.ToString(DisplayFormat), UnitString)
                        : Concat(FormattedValueString, UnitString);

                string Concat(string a, string b)
                    => string.IsNullOrEmpty(b)
                        ? a
                        : a + b;
            }
        }

        private double UiValue
        {
            get
            {
                if (_isInPopup == false)
                    return Value;

                var v = MakeValueFromString(_textBox.Text);

                return
                    v.Result == MakeValueResult.Ok || v.Result == MakeValueResult.Continue
                        ? v.Value
                        : Value;
            }
        }

        private Geometry ClipGeom
        {
            get
            {
                var size = new Size(ActualWidth, ActualHeight);
                if (_clipGeoms.TryGetValue(size, out var c))
                    return c;

                c = new RectangleGeometry
                {
                    RadiusX = CornerRadius,
                    RadiusY = CornerRadius,
                    Rect = new Rect(size)
                };

                c.Freeze();

                _clipGeoms.Add(size, c);

                return c;
            }
        }

        private static Geometry _DecSpinGeom;
        private static Geometry _IncSpinGeom;
        private static Brush _SpinBackground;

        private static readonly Dictionary<Size, RectangleGeometry> _clipGeoms =
            new Dictionary<Size, RectangleGeometry>();

        private static readonly Dictionary<(double X, double Y), TranslateTransform> _TranslateTransformCache =
            new Dictionary<(double X, double Y), TranslateTransform>();

        private static bool IsCtrl => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
    }
}