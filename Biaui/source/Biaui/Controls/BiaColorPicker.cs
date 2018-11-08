using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorPicker : Control
    {
        #region Red

        public double Red
        {
            get => _Red;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Red)
                    SetValue(RedProperty, value);
            }
        }

        private double _Red;

        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register(nameof(Red), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Red = (double) e.NewValue;
                        self.RgbToHsv();
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Green

        public double Green
        {
            get => _Green;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Green)
                    SetValue(GreenProperty, value);
            }
        }

        private double _Green;

        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register(nameof(Green), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Green = (double) e.NewValue;
                        self.RgbToHsv();
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Blue

        public double Blue
        {
            get => _Blue;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Blue)
                    SetValue(BlueProperty, value);
            }
        }

        private double _Blue;

        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register(nameof(Blue), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Blue = (double) e.NewValue;
                        self.RgbToHsv();
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Hue

        public double Hue
        {
            get => _Hue;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Hue)
                    SetValue(HueProperty, value);
            }
        }

        private double _Hue;

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Hue = (double) e.NewValue;
                        self.HsvToRgb();
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Saturation

        public double Saturation
        {
            get => _Saturation;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _Saturation)
                    SetValue(SaturationProperty, value);
            }
        }

        private double _Saturation;

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Saturation = (double) e.NewValue;
                        self.HsvToRgb();
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

        private double _Value;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Value = (double) e.NewValue;
                        self.HsvToRgb();
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._BorderColor = (Color) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        static BiaColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaColorPicker),
                new FrameworkPropertyMetadata(typeof(BiaColorPicker)));
        }

        private bool _isConverting;

        private void RgbToHsv()
        {
            if (_isConverting)
                return;

            _isConverting = true;

            // ReSharper disable CompareOfFloatsByEqualityOperator

            var max = Math.Max(Math.Max(Red, Green), Blue);
            var min = Math.Min(Math.Min(Red, Green), Blue);

            var h = 0.0;
            var s = 0.0;
            var v = max;

            if (max != min)
            {
                if (max == Red) h = 60.0 / 360 * (Green - Blue) / (max - min);
                else if (max == Green) h = 60.0 / 360 * (Blue - Red) / (max - min) + 120.0 / 360;
                else if (max == Blue) h = 60.0 / 360 * (Red - Green) / (max - min) + 240.0 / 360;

                s = (max - min) / max;
            }

            if (h < 0)
                h = h + 1;

            Hue = h;
            Saturation = s;
            Value = v;

            // ReSharper restore CompareOfFloatsByEqualityOperator

            _isConverting = false;
        }

        private void HsvToRgb()
        {
            if (_isConverting)
                return;

            _isConverting = true;

            // ReSharper disable CompareOfFloatsByEqualityOperator

            var h = Hue == 1 ? 0 : Hue;
            var s = Saturation;
            var v = Value;

            if (s == 0)
            {
                Red = v;
                Green = v;
                Blue = v;
            }
            else
            {
                var dh = Math.Floor(h / (60.0 / 360));
                var p = v * (1 - s);
                var q = v * (1 - s * (h / (60.0 / 360) - dh));
                var t = v * (1 - s * (1 - (h / (60.0 / 360) - dh)));

                switch (dh)
                {
                    case 0:
                        Red = v;
                        Green = t;
                        Blue = p;
                        break;

                    case 1:
                        Red = q;
                        Green = v;
                        Blue = p;
                        break;

                    case 2:
                        Red = p;
                        Green = v;
                        Blue = t;
                        break;

                    case 3:
                        Red = p;
                        Green = q;
                        Blue = v;
                        break;

                    case 4:
                        Red = t;
                        Green = p;
                        Blue = v;
                        break;

                    case 5:
                        Red = v;
                        Green = p;
                        Blue = q;
                        break;
                }
            }

            // ReSharper restore CompareOfFloatsByEqualityOperator

            _isConverting = false;
        }
    }
}