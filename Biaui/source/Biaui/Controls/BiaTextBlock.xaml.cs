using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTextBlock : FrameworkElement
    {
        #region Text

        public string? Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                    SetValue(TextProperty, value);
            }
        }

        private string? _Text;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBlock) s;
                        self._Text = (string) e.NewValue;
                        self.UpdateSize();
                    }));

        #endregion

        #region Foreground

        public Brush? Foreground
        {
            get => _Foreground;
            set
            {
                if (value != _Foreground)
                    SetValue(ForegroundProperty, value);
            }
        }

        private Brush? _Foreground;

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBlock) s;
                        self._Foreground = (Brush) e.NewValue;
                    }));

        #endregion
        
        #region TextTrimming

        public TextTrimmingMode TextTrimming
        {
            get => _TextTrimming;
            set
            {
                if (value != _TextTrimming)
                    SetValue(TextTrimmingProperty, Boxes.TextTrimming(value));
            }
        }

        private TextTrimmingMode _TextTrimming = TextTrimmingMode.Standard;

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register(
                nameof(TextTrimming),
                typeof(TextTrimmingMode),
                typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(
                    Boxes.TextTrimmingModeStandard,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBlock) s;
                        self._TextTrimming = (TextTrimmingMode) e.NewValue;
                    }));

        #endregion

        static BiaTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(typeof(BiaTextBlock)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            if (IsHitTestVisible)
                dc.DrawRectangle(Brushes.Transparent, null, this.RoundLayoutRenderRectangle(false));

            if (Text != null &&
                Foreground != null)
                DefaultTextRenderer.Instance.Draw(
                    this,
                    Text,
                    0, 0,
                    Foreground,
                    dc,
                    ActualWidth,
                    TextAlignment.Left,
                    TextTrimming,
                    true);
        }

        private double _textWidth;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size((constraint.Width, _textWidth).Min(), DefaultTextRenderer.Instance.FontHeight);
        }

        private void UpdateSize()
        {
            _textWidth = string.IsNullOrEmpty(Text) ? 0 : this.RoundLayoutValue(Math.Ceiling(DefaultTextRenderer.Instance.CalcWidth(Text)));
        }
    }
}