using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
                    (s, e) => { ((BiaNumberEditor) s)._IsReadOnly = (bool) e.NewValue; }));

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
                    (s, e) => { ((BiaNumberEditor) s)._BorderColor = (Color) e.NewValue; }));

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
                    (s, e) => { ((BiaNumberEditor) s)._SliderBrush = (Brush) e.NewValue; }));

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
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (((BiaNumberEditor) s)._Value == (double) e.NewValue)
                            return;

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
                    (s, e) => { ((BiaNumberEditor) s)._Caption = (string) e.NewValue; }));

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
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (((BiaNumberEditor) s)._SliderMinimum == (double) e.NewValue)
                            return;


                        ((BiaNumberEditor) s)._SliderMinimum = (double) e.NewValue;
                        ((BiaNumberEditor) s).InvalidateVisual();
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
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (((BiaNumberEditor) s)._SliderMaximum == (double) e.NewValue)
                            return;

                        ((BiaNumberEditor) s)._SliderMaximum = (double) e.NewValue;
                        ((BiaNumberEditor) s).InvalidateVisual();
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
                    (s, e) => { ((BiaNumberEditor) s)._DisplayFormat = (string) e.NewValue; }));

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
                    (s, e) => { ((BiaNumberEditor) s)._UnitString = (string) e.NewValue; }));

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

        private Pen BorderPen
        {
            get
            {
                if (_BorderPens.TryGetValue(BorderColor, out var p))
                    return p;

                var b = new SolidColorBrush(BorderColor);
                b.Freeze();

                p = new Pen(b, BorderSize);
                p.Freeze();

                _BorderPens.Add(BorderColor, p);

                return p;
            }
        }

        private RectangleGeometry ClipGeom
        {
            get
            {
                var size = new Size(ActualWidth, ActualHeight);
                if (_ClipGeoms.TryGetValue(size, out var c))
                    return c;

                c = new RectangleGeometry
                {
                    RadiusX = Constants.BasicCornerRadiusPrim,
                    RadiusY = Constants.BasicCornerRadiusPrim,
                    Rect = new Rect(size)
                };

                c.Freeze();

                _ClipGeoms.Add(size, c);

                return c;
            }
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
        private double SliderWidth => SliderMaximum - SliderMinimum;

        private const double BorderSize = 1.0;

        private static readonly Dictionary<Color, Pen> _BorderPens = new Dictionary<Color, Pen>();
        private static readonly Dictionary<Size, RectangleGeometry> _ClipGeoms = new Dictionary<Size, RectangleGeometry>();
    }
}