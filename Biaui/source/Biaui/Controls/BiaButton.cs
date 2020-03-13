using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaButton : BiaButtonBase
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
            DependencyProperty.Register(nameof(Content), typeof(string), typeof(BiaButton),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
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
                typeof(BiaButton),
                new FrameworkPropertyMetadata(
                    Boxes.TextTrimmingModeStandard,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._TextTrimming = (BiaTextTrimmingMode) e.NewValue;
                    }));

        #endregion

        static BiaButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaButton),
                new FrameworkPropertyMetadata(typeof(BiaButton)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            
            var rounder = new LayoutRounder(this);

            // 背景
            {
                if (NumberHelper.AreCloseZero(CornerRadius))
                    dc.DrawRectangle(
                        Background,
                        null,
                        rounder.RoundRenderRectangle(false));
                else
                    dc.DrawRoundedRectangle(
                        Background,
                        null,
                        rounder.RoundRenderRectangle(false),
                        CornerRadius,
                        CornerRadius);
            }

            // キャプション
            const double y = 4.0; // todo:正しく求める

            if (Content != null)
                if (Foreground != null)
                    DefaultTextRenderer.Instance.Draw(
                        this,
                        Content,
                        Constants.ButtonPaddingX,
                        y,
                        Foreground,
                        dc,
                        ActualWidth - Constants.ButtonPaddingX * 2.0,
                        TextAlignment.Center,
                        TextTrimming,
                        true);
        }

        private double _textWidth;

        protected override Size MeasureOverride(Size constraint)
        {
            var h = Height;
            if (double.IsNaN(h))
                h = Constants.BasicOneLineHeight;
            else if (double.IsInfinity(h))
                h = Constants.BasicOneLineHeight;

            return new Size((_textWidth, constraint.Width).Min(), h);
        }

        private void UpdateSize()
        {
            var w = Content == null
                ? 0.0
                : DefaultTextRenderer.Instance.CalcWidth(Content);

            _textWidth = Math.Ceiling(Constants.ButtonPaddingX + w + Constants.ButtonPaddingX);
        }
    }
}