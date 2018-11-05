using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            set => SetValue(IsReadOnlyProperty, value);
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
            set => SetValue(BorderColorProperty, value);
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
            set => SetValue(SliderBrushProperty, value);
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
            set => SetValue(ValueProperty, value);
        }

        private double _Value = default(double);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Value = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Caption

        public string Caption
        {
            get => _Caption;
            set => SetValue(CaptionProperty, value);
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
            set => SetValue(SliderMinimumProperty, value);
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
            set => SetValue(SliderMaximumProperty, value);
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
            set => SetValue(MinimumProperty, value);
        }

        private double _Minimum = default(double);

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Minimum = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Maximum

        public double Maximum
        {
            get => _Maximum;
            set => SetValue(MaximumProperty, value);
        }

        private double _Maximum = 100.0;

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.Double100,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._Maximum = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region DisplayFormat

        public string DisplayFormat
        {
            get => _DisplayFormat;
            set => SetValue(DisplayFormatProperty, value);
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
            set => SetValue(UnitStringProperty, value);
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
            set => SetValue(BackgroundProperty, value);
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
            set => SetValue(ForegroundProperty, value);
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
            set => SetValue(PaddingProperty, value);
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
            get => _mode;
            set => SetValue(ModeProperty, value);
        }

        private BiaNumberEditorMode _mode = BiaNumberEditorMode.Simple;

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(BiaNumberEditorMode), typeof(BiaNumberEditor),
                new PropertyMetadata(
                    Boxes.BiaNumberModeSimple,
                    (s, e) =>
                    {
                        var self = (BiaNumberEditor) s;
                        self._mode = (BiaNumberEditorMode) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Increment

        public double Increment
        {
            get => _Increment;
            set => SetValue(IncrementProperty, value);
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
                        self.InvalidateVisual();
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

        protected override void OnRender(DrawingContext dc)
        {
            dc.PushClip(ClipGeom);
            {
                DrawBackground(dc);

                if (Mode == BiaNumberEditorMode.Simple)
                    DrawSlider(dc);

                DrawText(dc);

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (IsReadOnly == false && IsEnabled && Increment != 0)
                    DrawSpin(dc);

                DrawBorder(dc);
            }
            dc.Pop();
        }

        private void DrawBackground(DrawingContext dc)
        {
            dc.DrawRoundedRectangle(
                Background,
                null,
                ActualRectangle, null,
                Constants.BasicCornerRadiusPrim, null,
                Constants.BasicCornerRadiusPrim, null);
        }

        private void DrawBorder(DrawingContext dc)
        {
            dc.DrawRoundedRectangle(
                null,
                BorderPen,
                ActualRectangle, null,
                Constants.BasicCornerRadiusPrim, null,
                Constants.BasicCornerRadiusPrim, null);
        }

        private void DrawSlider(DrawingContext dc)
        {
            if (SliderWidth <= 0.0f)
                return;

            var sliderBodyW = ActualWidth - BorderSize * 0.5 * 2;
            var w = (UiValue - ActualSliderMinimum) * sliderBodyW / SliderWidth;

            dc.DrawRectangle(SliderBrush, null, new Rect(BorderSize * 0.5, 0.0, w, ActualHeight));
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

                dc.PushTransform(new TranslateTransform(offsetX, offsetY));
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

                dc.PushTransform(new TranslateTransform(offsetX, offsetY));
                dc.DrawGeometry(
                    _mouseOverType == MouseOverType.IncSpin ? moBrush : Foreground, null, _IncSpinGeom);
                dc.Pop();
            }
        }

        private bool _isMouseDown;
        private bool _isMouseMoved;
        private Point _oldPos;
        private Point _mouseDownPos;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsReadOnly)
                return;

            _isMouseDown = true;
            _isMouseMoved = false;
            _oldPos = e.GetPosition(this);
            _mouseDownPos = _oldPos;

            CaptureMouse();

            // マウス可動域を設定
            if (Mode == BiaNumberEditorMode.Simple)
            {
                var p0 = new Point(0.0, 0.0);
                var p1 = new Point(ActualWidth + 1, ActualHeight + 1);
                var dp0 = PointToScreen(p0);
                var dp1 = PointToScreen(p1);
                var cr = new Win32RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
                ClipCursor(ref cr);
            }
        }

        private enum MouseOverType
        {
            None,
            DecSpin,
            IncSpin,
            Slider
        }

        private MouseOverType _mouseOverType;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var currentPos = e.GetPosition(this);

            {
                if (currentPos.X <= SpinWidth)
                    _mouseOverType = MouseOverType.DecSpin;
                else if (currentPos.X >= ActualWidth - SpinWidth)
                    _mouseOverType = MouseOverType.IncSpin;
                else
                    _mouseOverType = MouseOverType.Slider;
            }

            InvalidateVisual();

            if (_isMouseDown == false)
                return;

            if (IsReadOnly)
                return;

            // Down直後マウス移動ない場合でもOnMouseMoveが呼ばれることがあるため、判定する。
            if (currentPos == _oldPos)
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
                    SetCursorPos((int) p.X, (int) p.Y);
                    currentPos = _mouseDownPos;

                    break;
                }
            }

            _isMouseMoved = true;
            _mouseOverType = MouseOverType.Slider;
            _oldPos = currentPos;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsReadOnly == false)
            {
                ReleaseMouseCapture();

                switch (Mode)
                {
                    case BiaNumberEditorMode.Simple:
                        ClipCursor(IntPtr.Zero);
                        break;

                    case BiaNumberEditorMode.WideRange:
                    {
                        var p = PointToScreen(_mouseDownPos);
                        SetCursorPos((int) p.X, (int) p.Y);
                        GuiHelper.ShowCursor();

                        break;
                    }
                }

                _isMouseDown = false;
            }

            if (_isMouseMoved == false)
            {
                // Ctrl押下中は５倍速い
                var inc = IsCtrl ? Increment * 5 : Increment;

                var p = e.GetPosition(this);
                if (p.X <= SpinWidth && IsReadOnly == false)
                    AddValue(-inc);
                else if (p.X >= ActualWidth - SpinWidth && IsReadOnly == false)
                    AddValue(inc);
                else
                    ShowEditBox();
            }

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
                ClipCursor(IntPtr.Zero);
                GuiHelper.ShowCursor();
            }

            _mouseOverType = MouseOverType.None;

            InvalidateVisual();
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
                if (v.Ok)
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

        private (bool Ok, double Value) MakeValueFromString(string src)
        {
            if (double.TryParse(src, out var v))
                return (true, Math.Min(ActualMaximum, Math.Max(ActualMinimum, v)));

            return (false, default(double));
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
                    return FormattedValueString + UnitString;

                var v = MakeValueFromString(_textBox.Text);

                return
                    v.Ok
                        ? v.Value.ToString(DisplayFormat) + UnitString
                        : FormattedValueString + UnitString;
            }
        }

        private double UiValue
        {
            get
            {
                if (_isInPopup == false)
                    return Value;

                var v = MakeValueFromString(_textBox.Text);

                return v.Ok ? v.Value : Value;
            }
        }

        private Pen BorderPen
        {
            get
            {
                if (_borderPens.TryGetValue(BorderColor, out var p))
                    return p;

                var b = new SolidColorBrush(BorderColor);
                b.Freeze();

                p = new Pen(b, BorderSize);
                p.Freeze();

                _borderPens.Add(BorderColor, p);

                return p;
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
                    RadiusX = Constants.BasicCornerRadiusPrim,
                    RadiusY = Constants.BasicCornerRadiusPrim,
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

        private const double BorderSize = 1.0;
        private static readonly Dictionary<Color, Pen> _borderPens = new Dictionary<Color, Pen>();

        private static readonly Dictionary<Size, RectangleGeometry> _clipGeoms =
            new Dictionary<Size, RectangleGeometry>();

        private static bool IsCtrl => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(ref Win32RECT lpWin32Rect);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(IntPtr ptr);

        public struct Win32RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Win32RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }
    }
}