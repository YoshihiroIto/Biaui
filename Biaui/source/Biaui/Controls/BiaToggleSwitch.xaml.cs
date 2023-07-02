using System.Windows;
using System.Windows.Controls;
using Biaui.Controls.Converters;

namespace Biaui.Controls;

public class BiaToggleSwitch : ContentControl
{
    public static BoolInverseConverter InverseConverter { get; } = new BoolInverseConverter();

    #region IsChecked

    public bool IsChecked
    {
        get => _IsChecked;
        set
        {
            if (value != _IsChecked)
                SetValue(IsCheckedProperty, Boxes.Bool(value));
        }
    }

    private bool _IsChecked;

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(BiaToggleSwitch),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (s, e) =>
                {
                    var self = (BiaToggleSwitch) s;
                    self._IsChecked = (bool) e.NewValue;
                })
            {
                BindsTwoWayByDefault = true
            }
        );

    #endregion

    #region EnabledLabel
        
    public string? EnabledLabel
    {
        get => _EnabledLabel;
        set
        {
            if (value != _EnabledLabel)
                SetValue(EnabledLabelProperty, value);
        }
    }
        
    private string? _EnabledLabel;
        
    public static readonly DependencyProperty EnabledLabelProperty =
        DependencyProperty.Register(
            nameof(EnabledLabel),
            typeof(string),
            typeof(BiaToggleSwitch),
            new PropertyMetadata(
                default,
                (s, e) =>
                {
                    var self = (BiaToggleSwitch) s;
                    self._EnabledLabel = (string)e.NewValue;
                }));
        
    #endregion

    #region DisabledLabel
        
    public string? DisabledLabel
    {
        get => _DisabledLabel;
        set
        {
            if (value != _DisabledLabel)
                SetValue(DisabledLabelProperty, value);
        }
    }
        
    private string? _DisabledLabel;
        
    public static readonly DependencyProperty DisabledLabelProperty =
        DependencyProperty.Register(
            nameof(DisabledLabel),
            typeof(string),
            typeof(BiaToggleSwitch),
            new PropertyMetadata(
                default,
                (s, e) =>
                {
                    var self = (BiaToggleSwitch) s;
                    self._DisabledLabel = (string)e.NewValue;
                }));
        
    #endregion

    static BiaToggleSwitch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaToggleSwitch),
            new FrameworkPropertyMetadata(typeof(BiaToggleSwitch)));
    }
}