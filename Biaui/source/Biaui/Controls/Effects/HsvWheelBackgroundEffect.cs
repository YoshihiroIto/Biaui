using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Biaui.Internals;

namespace Biaui.Controls.Effects
{
    internal class HsvWheelBackgroundEffect : ShaderEffect
    {
        internal HsvWheelBackgroundEffect()
        {
            var ps = new PixelShader
            {
                UriSource = new Uri(
                    "pack://application:,,,/Biaui;component/Controls/Effects/HsvWheelBackgroundEffect.ps")
            };

            ps.Freeze();

            PixelShader = ps;
            UpdateShaderValue(ValueProperty);
            UpdateShaderValue(BorderColorProperty);
        }

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
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(HsvWheelBackgroundEffect),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (HsvWheelBackgroundEffect) s;
                        self._Value = (double) e.NewValue;

                        PixelShaderConstantCallback(0)(s, e);
                    }));

        #endregion

        #region AspectRatioCorrectionX

        public double AspectRatioCorrectionX
        {
            get => _AspectRatioCorrectionX;
            set
            {
                if (NumberHelper.AreClose(value, _AspectRatioCorrectionX) == false)
                    SetValue(AspectRatioCorrectionXProperty, value);
            }
        }

        private double _AspectRatioCorrectionX = 1;

        public static readonly DependencyProperty AspectRatioCorrectionXProperty =
            DependencyProperty.Register(nameof(AspectRatioCorrectionX), typeof(double),
                typeof(HsvWheelBackgroundEffect),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (HsvWheelBackgroundEffect) s;
                        self._AspectRatioCorrectionX = (double) e.NewValue;

                        PixelShaderConstantCallback(1)(s, e);
                    }));

        #endregion

        #region AspectRatioCorrectionY

        public double AspectRatioCorrectionY
        {
            get => _AspectRatioCorrectionY;
            set
            {
                if (NumberHelper.AreClose(value, _AspectRatioCorrectionY) == false)
                    SetValue(AspectRatioCorrectionYProperty, value);
            }
        }

        private double _AspectRatioCorrectionY = 1;

        public static readonly DependencyProperty AspectRatioCorrectionYProperty =
            DependencyProperty.Register(nameof(AspectRatioCorrectionY), typeof(double),
                typeof(HsvWheelBackgroundEffect),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (HsvWheelBackgroundEffect) s;
                        self._AspectRatioCorrectionY = (double) e.NewValue;

                        PixelShaderConstantCallback(2)(s, e);
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(HsvWheelBackgroundEffect),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (HsvWheelBackgroundEffect) s;
                        self._BorderColor = (Color) e.NewValue;

                        PixelShaderConstantCallback(3)(s, e);
                    }));

        #endregion
    }
}