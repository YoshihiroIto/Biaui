using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanel : Control
    {
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaNodePanel),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaNodePanel) s;
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
                    SetValue(CornerRadiusProperty, value);
            }
        }

        private double _CornerRadius = default(double);

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaNodePanel),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaNodePanel) s;
                        self._CornerRadius = (double) e.NewValue;
                    }));

        #endregion

        #region Background
        
        public new Brush Background
        {
            get => _Background;
            set
            {
                if (value != _Background)
                    SetValue(BackgroundProperty, value);
            }
        }
        
        private Brush _Background = default(Brush);
        
        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaNodePanel),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaNodePanel) s;
                        self._Background = (Brush)e.NewValue;
                    }));
        
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawBackground(dc);

            var isCornerRadiusZero = NumberHelper.AreCloseZero(CornerRadius);

            if (isCornerRadiusZero == false)
                dc.PushClip(
                    Caches.GetClipGeom(ActualWidth, ActualHeight, CornerRadius, true));
            {
                DrawName(dc);
            }
            if (isCornerRadiusZero == false)
                dc.Pop();

            DrawBorder(dc);
        }

        private void DrawBackground(DrawingContext dc)
        {
            var brush = Background;

            if (NumberHelper.AreCloseZero(CornerRadius))
                dc.DrawRectangle(
                    brush,
                    null,
                    this.RoundLayoutActualRectangle(true));
            else
                dc.DrawRoundedRectangle(
                    brush,
                    null,
                    this.RoundLayoutActualRectangle(true),
                    CornerRadius,
                    CornerRadius);
        }

        private void DrawBorder(DrawingContext dc)
        {
            if (NumberHelper.AreCloseZero(CornerRadius))
                dc.DrawRectangle(
                    Brushes.Transparent,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutActualRectangle(true)
                );
            else
                dc.DrawRoundedRectangle(
                    Brushes.Transparent,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutActualRectangle(true),
                    CornerRadius,
                    CornerRadius);
        }

        private void DrawName(DrawingContext dc)
        {
            var i = DataContext as INodeItem;

            if (i == null)
                return;

            var b = Caches.GetSolidColorBrush(i.TitleColor);
            var y = FrameworkElementHelper.RoundLayoutValue(32);

            dc.DrawRectangle(b, null, new Rect(0, 0, this.RoundLayoutActualWidth(true), y));
            dc.DrawLine(this.GetBorderPen(BorderColor), new Point(0, y), new Point(ActualWidth, y));

            var name = i.Name;

            if (string.IsNullOrEmpty(name) == false)
                TextRenderer.Default.Draw(name, 0, 8, Brushes.WhiteSmoke, dc, ActualWidth, TextAlignment.Center);
        }
    }
}