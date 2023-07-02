using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls;

public class BiaHsvWheelCursor : FrameworkElement
{
    #region Hue

    public double Hue
    {
        get => _Hue;
        set
        {
            if (NumberHelper.AreClose(value, _Hue) == false)
                SetValue(HueProperty, Boxes.Double(value));
        }
    }

    private double _Hue;

    public static readonly DependencyProperty HueProperty =
        DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvWheelCursor),
            new FrameworkPropertyMetadata(
                Boxes.Double0,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaHsvWheelCursor) s;
                    self._Hue = (double) e.NewValue;
                }));

    #endregion

    #region Saturation

    public double Saturation
    {
        get => _Saturation;
        set
        {
            if (NumberHelper.AreClose(value, _Saturation) == false)
                SetValue(SaturationProperty, Boxes.Double(value));
        }
    }

    private double _Saturation;

    public static readonly DependencyProperty SaturationProperty =
        DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvWheelCursor),
            new FrameworkPropertyMetadata(
                Boxes.Double0,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaHsvWheelCursor) s;
                    self._Saturation = (double) e.NewValue;
                }));

    #endregion

    #region IsReadOnly

    public bool IsReadOnly
    {
        get => _IsReadOnly;
        set
        {
            if (value != _IsReadOnly)
                SetValue(IsReadOnlyProperty, Boxes.Bool(value));
        }
    }

    private bool _IsReadOnly;

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvWheelCursor),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaHsvWheelCursor) s;
                    self._IsReadOnly = (bool) e.NewValue;
                }));

    #endregion

    static BiaHsvWheelCursor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHsvWheelCursor),
            new FrameworkPropertyMetadata(typeof(BiaHsvWheelCursor)));
    }

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly PropertyChangeNotifier _isEnabledChangeNotifier;

    public BiaHsvWheelCursor()
    {
        IsHitTestVisible = false;

        _isEnabledChangeNotifier = new PropertyChangeNotifier(this, IsEnabledProperty);
        _isEnabledChangeNotifier.ValueChanged += (_, __) => InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (ActualWidth <= 1d ||
            ActualHeight <= 1d)
            return;

        var rounder = new LayoutRounder(this);

        // Cursor
        this.DrawPointCursor(rounder, dc, MakeCursorRenderPos(rounder), IsEnabled, IsReadOnly);
    }

    private ImmutableVec2_double MakeCursorRenderPos(in LayoutRounder rounder)
        => MakeCursorRenderPos(rounder, ActualWidth, ActualHeight, Hue, Saturation);

    internal static ImmutableVec2_double MakeCursorRenderPos(
        in LayoutRounder rounder,
        double actualWidth,
        double actualHeight,
        double hue,
        double saturation)
    {
        hue = NumberHelper.Clamp01(hue);
        saturation = NumberHelper.Clamp01(saturation);

        var bw = rounder.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);
        var w = rounder.RoundLayoutValue(actualWidth - bw * 2d);
        var h = rounder.RoundLayoutValue(actualHeight - bw * 2d);

        var r = hue * 2d * Math.PI;

        var (cx, cy) = MakeAspectRatioCorrection(actualWidth, actualHeight);

        var x = bw + Math.Cos(r) * saturation * (w * 0.5d) / cx + w * 0.5d;
        var y = bw + Math.Sin(r) * saturation * (h * 0.5d) / cy + h * 0.5d;

        return new ImmutableVec2_double(rounder.RoundLayoutValue(x), rounder.RoundLayoutValue(y));
    }

    internal static (double X, double Y) MakeAspectRatioCorrection(double actualWidth, double actualHeight)
        => actualWidth > actualHeight
            ? (actualWidth / actualHeight, 1d)
            : (1d, actualHeight / actualWidth);
}