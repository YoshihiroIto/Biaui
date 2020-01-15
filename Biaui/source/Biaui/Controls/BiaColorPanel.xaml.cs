using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorPanel : FrameworkElement
    {
        #region Value

        public Color Value
        {
            get => _Value;
            set
            {
                if (value.Equals(_Value) == false)
                    SetValue(ValueProperty, value);
            }
        }

        private Color _Value = Colors.White;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Color), typeof(BiaColorPanel),
                new FrameworkPropertyMetadata(
                    Boxes.ColorWhite,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorPanel) s;
                        self._Value = (Color) e.NewValue;
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

        private Color _BorderColor = Colors.Transparent;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaColorPanel),
                new FrameworkPropertyMetadata(
                    Boxes.ColorTransparent,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorPanel) s;
                        self._BorderColor = (Color) e.NewValue;
                    }));

        #endregion

        #region CornerRadius

        public double CornerRadius
        {
            get => _CornerRadius;
            set
            {
                if (NumberHelper.AreClose(value, _CornerRadius) == false)
                    SetValue(CornerRadiusProperty, Boxes.Double(value));
            }
        }

        private double _CornerRadius;

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaColorPanel),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorPanel) s;
                        self._CornerRadius = (double) e.NewValue;
                    }));

        #endregion

        static BiaColorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaColorPanel),
                new FrameworkPropertyMetadata(typeof(BiaColorPanel)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var rect = this.RoundLayoutRenderRectangle(true);

            var borderPen = this.GetBorderPen(BorderColor.ToImmutableByteColor());

            if (IsEnabled)
            {
                var brush = Caches.GetSolidColorBrush(Value.ToImmutableByteColor());

                if (NumberHelper.AreCloseZero(CornerRadius))
                {
                    if (Value.A != 0xFF)
                        dc.DrawRectangle(Constants.CheckerBrush, null, rect);

                    dc.DrawRectangle(brush, borderPen, rect);
                }
                else
                {
                    if (Value.A != 0xFF)
                        dc.DrawRoundedRectangle(Constants.CheckerBrush, null, rect, CornerRadius, CornerRadius);

                    dc.DrawRoundedRectangle(brush, borderPen, rect, CornerRadius, CornerRadius);
                }
            }
            else
            {
                if (NumberHelper.AreCloseZero(CornerRadius))
                    dc.DrawRectangle(null, borderPen, rect);
                else
                    dc.DrawRoundedRectangle(null, borderPen, rect, CornerRadius, CornerRadius);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // todo:DPI変更時に再描画が行われないため明示的に指示している。要調査。
            InvalidateVisual();

            return new Size(ActualWidth, ActualHeight);
        }
    }
}