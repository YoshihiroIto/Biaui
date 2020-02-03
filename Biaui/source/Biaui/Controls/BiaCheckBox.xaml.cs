using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaCheckBox : BiaToggleButton
    {
        #region BoxBorderColor

        public ByteColor BoxBorderColor
        {
            get => _boxBorderColor;
            set
            {
                if (value != _boxBorderColor)
                    SetValue(BoxBorderColorProperty, value);
            }
        }

        private ByteColor _boxBorderColor = ByteColor.Transparent;

        public static readonly DependencyProperty BoxBorderColorProperty =
            DependencyProperty.Register(nameof(BoxBorderColor), typeof(ByteColor), typeof(BiaCheckBox),
                new FrameworkPropertyMetadata(
                    Boxes.ByteColorTransparent,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaCheckBox) s;
                        self._boxBorderColor = (ByteColor) e.NewValue;
                    }));

        #endregion

        #region MarkBrush

        public Brush? MarkBrush
        {
            get => _MarkBrush;
            set
            {
                if (value != _MarkBrush)
                    SetValue(MarkBrushProperty, value);
            }
        }

        private Brush? _MarkBrush;

        public static readonly DependencyProperty MarkBrushProperty =
            DependencyProperty.Register(nameof(MarkBrush), typeof(Brush), typeof(BiaCheckBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaCheckBox) s;
                        self._MarkBrush = (Brush) e.NewValue;
                    }));

        #endregion
        
        static BiaCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaCheckBox),
                new FrameworkPropertyMetadata(typeof(BiaCheckBox)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawBackground(dc);

            // ボックス
            dc.DrawRoundedRectangle(
                Background,
                this.GetBorderPen(BoxBorderColor),
                this.RoundLayoutRect(
                    0 + FrameworkElementExtensions.BorderHalfWidth, 
                    2 + FrameworkElementExtensions.BorderHalfWidth,
                    16 - FrameworkElementExtensions.BorderWidth,
                    16 - FrameworkElementExtensions.BorderWidth),
                CornerRadius, 
                CornerRadius);

            // マーク
            if (IsChecked && IsPressed == false)
                dc.DrawGeometry(MarkBrush, null, _markGeom);

            // キャプション
            DrawCaption(dc);
        }

        protected void DrawBackground(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, this.RoundLayoutRenderRectangle(false));
        }

        protected void DrawCaption(DrawingContext dc)
        {
            DrawCaption(dc, 16 + 4, 2);
        }

        protected void DrawCaption(DrawingContext dc, double x, double y)
        {
            if (Content == null)
                return;

            if (Foreground == null)
                return;

            DefaultTextRenderer.Instance.Draw(this, Content, x, y, Foreground, dc, ActualWidth - x, TextAlignment.Left, TextTrimming, true);
        }

        private static readonly Geometry _markGeom = Geometry.Parse("M7.7 15.1L7.52 15.1L3 11.19L4.28 9.52L7.26 12.09L11.33 6L12.97 7.22L7.7 15.1Z");
    }
}