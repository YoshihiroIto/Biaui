using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaNumberEditor : Control
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
                    Colors.Red,
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
                        ((BiaNumberEditor) s)._Value = (double) e.NewValue;
                        ((BiaNumberEditor) s).InvalidateVisual();
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
            var w = (Value - SliderMinimum) * sliderBodyW / SliderWidth;

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
                Value.ToString(DisplayFormat) + UnitString,
                Padding.Left,
                Padding.Top,
                Foreground,
                dc,
                ActualWidth - Padding.Left - Padding.Right,
                TextAlignment.Right
            );
        }

        private bool _isMouseDown;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsReadOnly)
                return;

            _isMouseDown = true;
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
                var cr = new Win32RECT((int)dp0.X, (int)dp0.Y, (int)dp1.X, (int)dp1.Y);
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

            // 0から1
            var xr = Math.Min(Math.Max(0, e.GetPosition(this).X), ActualWidth) / ActualWidth;

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

            //Cursor = Cursors.Arrow;
            //var x = Math.Min(Math.Max(0, e.GetPosition(this).X), ActualWidth);
            //var y = ActualHeight * 0.5;
            //var p = PointToScreen(new Point(x, y));
            //SetCursorPos((int) p.X, (int) p.Y);
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

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
        private double SliderWidth => SliderMaximum - SliderMinimum;

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