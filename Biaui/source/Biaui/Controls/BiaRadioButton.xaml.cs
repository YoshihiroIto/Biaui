using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaRadioButton : BiaToggleButton
    {
        #region MarkBorderColor

        public ByteColor MarkBorderColor
        {
            get => _markBorderColor;
            set
            {
                if (value != _markBorderColor)
                    SetValue(MarkBorderColorProperty, value);
            }
        }

        private ByteColor _markBorderColor;

        public static readonly DependencyProperty MarkBorderColorProperty =
            DependencyProperty.Register(nameof(MarkBorderColor), typeof(ByteColor), typeof(BiaRadioButton),
                new FrameworkPropertyMetadata(
                    Boxes.ByteColorTransparent,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaRadioButton) s;
                        self._markBorderColor = (ByteColor) e.NewValue;
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
            DependencyProperty.Register(nameof(MarkBrush), typeof(Brush), typeof(BiaRadioButton),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaRadioButton) s;
                        self._MarkBrush = (Brush) e.NewValue;
                    }));

        #endregion
        
        static BiaRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaRadioButton),
                new FrameworkPropertyMetadata(typeof(BiaRadioButton)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            
            var rounder = new LayoutRounder(this);

            dc.DrawRectangle(Brushes.Transparent, null, rounder.RoundRenderRectangle(false));

            if (IsEnabled)
            {
                var color =
                    IsMouseOver
                        ? MarkBorderColor
                        : (Background as SolidColorBrush)?.Color.ToByteColor() ?? MarkBorderColor;


                dc.DrawEllipse(
                    IsPressed
                        ? MarkBrush
                        : Background,
                    rounder.GetBorderPen(color),
                    new Point(8, 10),
                    7, 7);
            }
            else
            {
                dc.DrawEllipse(
                    null,
                    rounder.GetBorderPen(MarkBorderColor),
                    new Point(8, 10),
                    7, 7);
            }

            if (IsChecked)
            {
                dc.DrawEllipse(
                    MarkBrush,
                    null,
                    new Point(8, 10),
                    5, 5);
            }

            // キャプション
            if (Content != null &&
                Foreground != null)
                DefaultTextRenderer.Instance.Draw(this, Content, 16 + 4, 2, Foreground, dc, ActualWidth, TextAlignment.Left, TextTrimming, true);
        }
    }
}