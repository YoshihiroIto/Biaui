using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaHyperlinkButton : BiaButtonBase
    {
        #region Content

        public string? Content
        {
            get => _content;
            set
            {
                if (value != _content)
                    SetValue(ContentProperty, value);
            }
        }

        private string? _content;

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(string), typeof(BiaHyperlinkButton),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHyperlinkButton) s;
                        self._content = e.NewValue?.ToString() ?? "";
                        self.UpdateSize();
                    }));

        #endregion

        #region TextTrimming

        public BiaTextTrimmingMode TextTrimming
        {
            get => _TextTrimming;
            set
            {
                if (value != _TextTrimming)
                    SetValue(TextTrimmingProperty, Boxes.TextTrimming(value));
            }
        }

        private BiaTextTrimmingMode _TextTrimming = BiaTextTrimmingMode.Standard;

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register(
                nameof(TextTrimming),
                typeof(BiaTextTrimmingMode),
                typeof(BiaHyperlinkButton),
                new FrameworkPropertyMetadata(
                    Boxes.TextTrimmingModeStandard,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHyperlinkButton) s;
                        self._TextTrimming = (BiaTextTrimmingMode) e.NewValue;
                    }));

        #endregion

        static BiaHyperlinkButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHyperlinkButton),
                new FrameworkPropertyMetadata(typeof(BiaHyperlinkButton)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var rounder = new LayoutRounder(this);
            dc.DrawRectangle(Background ?? Brushes.Transparent, null, rounder.RoundRenderRectangle(false));

            if (Content != null &&
                Foreground != null)
            {
                var width = DefaultTextRenderer.Instance.Draw(
                    this,
                    Content,
                    0, 0,
                    Foreground,
                    dc,
                    ActualWidth,
                    TextAlignment.Left,
                    TextTrimming,
                    true);

                if (Foreground is SolidColorBrush brush)
                {
                    var y = rounder.RoundLayoutValue(ActualHeight - 2d + FrameworkElementExtensions.BorderHalfWidth);
                    var pen = Caches.GetPen(brush.Color.ToByteColor(), 1d);

                    var x = rounder.RoundLayoutValue(Math.Round(width));

                    dc.DrawLine(
                        pen,
                        new Point(0d, y), 
                        new Point(x, y));
                }
            }
        }

        private double _textWidth;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size((constraint.Width, _textWidth).Min(), DefaultTextRenderer.Instance.FontHeight);
        }

        private void UpdateSize()
        {
            var rounder = new LayoutRounder(this);
            
            _textWidth = string.IsNullOrEmpty(Content) ? 0 : rounder.RoundLayoutValue(Math.Ceiling(DefaultTextRenderer.Instance.CalcWidth(Content)));
        }
    }
}