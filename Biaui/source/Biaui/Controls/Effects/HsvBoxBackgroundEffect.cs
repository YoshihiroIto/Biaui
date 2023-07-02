using System;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using Biaui.Internals;

namespace Biaui.Controls.Effects;

internal class HsvBoxBackgroundEffect : ShaderEffect
{
    static HsvBoxBackgroundEffect()
    {
        _PixelShader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Biaui;component/Controls/Effects/HsvBoxBackgroundEffect.ps")
        };

        _PixelShader.Freeze();
    }

    // ReSharper disable once InconsistentNaming
    private static readonly PixelShader _PixelShader;

    internal HsvBoxBackgroundEffect()
    {
        PixelShader = _PixelShader;
        UpdateShaderValue(ValueProperty);
        UpdateShaderValue(IsEnabledProperty);
        UpdateShaderValue(DisableColorProperty);
    }

    #region Value

    public double Value
    {
        get => _Value;
        set
        {
            if (NumberHelper.AreClose(value, Value) == false)
                SetValue(ValueProperty, Boxes.Double(value));
        }
    }

    private double _Value;

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(HsvBoxBackgroundEffect),
            new PropertyMetadata(
                Boxes.Double0,
                (s, e) =>
                {
                    var self = (HsvBoxBackgroundEffect) s;
                    self._Value = (double) e.NewValue;

                    PixelShaderConstantCallback(0)(s, e);
                }));

    #endregion

    #region IsEnabled

    public float IsEnabled
    {
        get => _IsEnabled;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value != _IsEnabled)
                SetValue(IsEnabledProperty, Boxes.Float(value));
        }
    }

    private float _IsEnabled = 1.0f;

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(float), typeof(HsvBoxBackgroundEffect),
            new PropertyMetadata(
                Boxes.Float1,
                (s, e) =>
                {
                    var self = (HsvBoxBackgroundEffect) s;
                    self._IsEnabled = (float) e.NewValue;

                    PixelShaderConstantCallback(6)(s, e);
                }));

    #endregion

    #region DisableColor

    public Point3D DisableColor
    {
        get => _DisableColor;
        set
        {
            if (value != _DisableColor)
                SetValue(DisableColorProperty, value);
        }
    }

    private Point3D _DisableColor = new Point3D(1.0, 0.0, 0.0);

    public static readonly DependencyProperty DisableColorProperty =
        DependencyProperty.Register(nameof(DisableColor), typeof(Point3D), typeof(HsvBoxBackgroundEffect),
            new PropertyMetadata(
                Boxes.Point3dRed,
                (s, e) =>
                {
                    var self = (HsvBoxBackgroundEffect) s;
                    self._DisableColor = (Point3D) e.NewValue;

                    PixelShaderConstantCallback(7)(s, e);
                }));

    #endregion
}