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

        static BiaNumberEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNumberEditor),
                new FrameworkPropertyMetadata(typeof(BiaNumberEditor)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawBackground(dc);
            DrawSlider(dc);
            DrawBorder(dc);
            DrawText(dc);
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
            var w = (UiValue - SliderMinimum) * sliderBodyW / SliderWidth;

            dc.PushClip(ClipGeom);
            dc.DrawRectangle(SliderBrush, null, new Rect(BorderSize * 0.5, 0.0, w, ActualHeight));
            dc.Pop();
        }

        private void DrawText(DrawingContext dc)
        {
            TextRenderer.Default.Draw(
                Caption,
                Padding.Left,
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
                ActualWidth - Padding.Left - Padding.Right,
                TextAlignment.Right
            );
        }

        private bool _isMouseDown;
        private bool _isMouseMoved;
        private Point _oldPos;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsReadOnly)
                return;

            _isMouseDown = true;
            _isMouseMoved = false;
            _oldPos = e.GetPosition(this);

            CaptureMouse();
            //Cursor = Cursors.None;

#if false
// 現在値位置にマウス位置を位置させる
            {
                var pos = e.GetPosition(this);

                var w = (Value - SliderMinimum) * ActualWidth / SliderWidth;

                var xr = Math.Min(Math.Max(0, w), ActualWidth) / ActualWidth;
                var x = ActualWidth * xr;

                var p = new Point(x, pos.Y);
                var dp = PointToScreen(p);

                SetCursorPos((int) dp.X, (int) dp.Y);
            }
#endif

            // マウス可動域を設定
            {
                var p0 = new Point(0.0, 0.0);
                var p1 = new Point(ActualWidth + 1, ActualHeight + 1);
                var dp0 = PointToScreen(p0);
                var dp1 = PointToScreen(p1);
                var cr = new Win32RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
                ClipCursor(ref cr);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown == false)
                return;

            if (IsReadOnly)
                return;

            var currentPos = e.GetPosition(this);

            // Down直後マウス移動ない場合でもOnMouseMoveが呼ばれることがあるため、判定する。
            if (currentPos == _oldPos)
                return;

            _isMouseMoved = true;
            _oldPos = currentPos;

            // 0から1
            var xr = Math.Min(Math.Max(0, currentPos.X), ActualWidth) / ActualWidth;

            Value = SliderWidth * xr;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsReadOnly)
                return;

            _isMouseDown = false;
            ReleaseMouseCapture();
            ClipCursor(IntPtr.Zero);

#if false
            Cursor = Cursors.Arrow;
            var x = Math.Min(Math.Max(0, e.GetPosition(this).X), ActualWidth);
            var y = ActualHeight * 0.5;
            var p = PointToScreen(new Point(x, y));
            SetCursorPos((int) p.X, (int) p.Y);
#endif

            if (_isMouseMoved == false)
                ShowEditBox();
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
            }

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

        private void ShowEditBox()
        {
            _textBox.Width = ActualWidth;
            _textBox.Height = ActualHeight;
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
                return (true, Math.Min(Maximum, Math.Max(Minimum, v)));

            return (false, default(double));
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
        private double SliderWidth => SliderMaximum - SliderMinimum;
        private string FormattedValueString => Value.ToString(DisplayFormat);

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

        private RectangleGeometry ClipGeom
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

        private const double BorderSize = 1.0;
        private static readonly Dictionary<Color, Pen> _borderPens = new Dictionary<Color, Pen>();

        private static readonly Dictionary<Size, RectangleGeometry> _clipGeoms =
            new Dictionary<Size, RectangleGeometry>();

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