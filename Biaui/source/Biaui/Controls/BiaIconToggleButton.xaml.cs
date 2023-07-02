using System.Windows;

namespace Biaui.Controls;

public class BiaIconToggleButton : BiaIconButton
{
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
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(BiaIconToggleButton),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaIconToggleButton) s;
                    self._IsChecked = (bool) e.NewValue;
                })
            {
                BindsTwoWayByDefault = true
            }
        );

    #endregion

    static BiaIconToggleButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaIconToggleButton),
            new FrameworkPropertyMetadata(typeof(BiaIconToggleButton)));
    }

    protected override void Clicked()
    {
        IsChecked = !IsChecked;

        base.Clicked();
    }
}