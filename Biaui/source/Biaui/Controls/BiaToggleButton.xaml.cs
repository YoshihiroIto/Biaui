using System;
using System.Windows;

namespace Biaui.Controls
{
    public enum BiaToggleButtonBehavior
    {
        Normal,
        RadioButton
    }

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

        private bool _IsChecked;

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

        #region Behavior

        public BiaToggleButtonBehavior Behavior
        {
            get => _Behavior;
            set
            {
                if (value != _Behavior)
                    SetValue(BehaviorProperty, Boxes.ToggleButtonBehavior(value));
            }
        }

        private BiaToggleButtonBehavior _Behavior = BiaToggleButtonBehavior.Normal;

        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.Register(
                nameof(Behavior),
                typeof(BiaToggleButtonBehavior),
                typeof(BiaToggleButton),
                new FrameworkPropertyMetadata(
                    Boxes.ToggleButtonBehaviorNormal,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaToggleButton) s;
                        self._Behavior = (BiaToggleButtonBehavior) e.NewValue;
                    }));

        #endregion

        static BiaToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaToggleButton),
                new FrameworkPropertyMetadata(typeof(BiaToggleButton)));
        }

        protected override void Clicked()
        {
            switch (Behavior)
            {
                case BiaToggleButtonBehavior.Normal:
                    IsChecked = !IsChecked;
                    base.InvokeClicked();

                    break;

                case BiaToggleButtonBehavior.RadioButton:
                    if (IsChecked)
                        return;

                    IsChecked = true;

                    base.InvokeClicked();

                    if (IsChecked)
                        UpdateSibling();

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateSibling()
        {
            var parent = Parent;
            if (parent is null)
                return;

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is BiaToggleButton toggleButton &&
                    toggleButton != this)
                    toggleButton.IsChecked = false;
            }
        }
    }
}