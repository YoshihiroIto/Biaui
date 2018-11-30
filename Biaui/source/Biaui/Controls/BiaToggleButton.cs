using System.Windows;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaToggleButton : BiaButton
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

        private bool _IsChecked = default(bool);

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(BiaToggleButton),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaToggleButton) s;
                        self._IsChecked = (bool) e.NewValue;
                    })
                {
                    BindsTwoWayByDefault = true
                }
            );

        #endregion

        static BiaToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaToggleButton),
                new FrameworkPropertyMetadata(typeof(BiaToggleButton)));
        }

        protected override void Clicked()
        {
            IsChecked = !IsChecked;

            base.Clicked();
        }
    }
}