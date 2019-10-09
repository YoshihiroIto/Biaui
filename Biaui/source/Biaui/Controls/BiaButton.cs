using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaButton : BiaButtonBase
    {
        #region Content

        public string Content
        {
            get => _content;
            set
            {
                if (value != _content)
                    SetValue(ContentProperty, value);
            }
        }

        private string _content;

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
                        self._content = (string) e.NewValue;
                        self.UpdateSize();
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

            // 背景
            {
                if (NumberHelper.AreCloseZero(CornerRadius))
                    dc.DrawRectangle(
                        Background,
                        null,
                        this.RoundLayoutRenderRectangle(false));
                else
                    dc.DrawRoundedRectangle(
                        Background,
                        null,
                        this.RoundLayoutRenderRectangle(false),
                        CornerRadius,
                        CornerRadius);
            }

            // キャプション
            const double y = 4.0; // todo:正しく求める

            TextRenderer.Default.Draw(
                this,
                Content,
                Constants.ButtonPaddingX,
                y,
                Foreground,
                dc,
                ActualWidth - Constants.ButtonPaddingX * 2.0,
                TextAlignment.Center);
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
            var w = TextRenderer.Default.CalcWidth(Content);

            _textWidth = Math.Ceiling(Constants.ButtonPaddingX + w + Constants.ButtonPaddingX);
        }
    }
}