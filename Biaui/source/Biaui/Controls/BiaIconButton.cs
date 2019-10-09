using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaIconButton : BiaButtonBase
    {
        #region Content

        public Geometry Content
        {
            get => _content;
            set
            {
                if (value != _content)
                    SetValue(ContentProperty, value);
            }
        }

        private Geometry _content;

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(Geometry), typeof(BiaIconButton),
                new FrameworkPropertyMetadata(
                    default(Geometry),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaIconButton) s;
                        self._content = (Geometry) e.NewValue;
                    }));

        #endregion

        #region ContentSize

        public double ContentSize
        {
            get => _contentSize;
            set
            {
                if (NumberHelper.AreClose(value, _contentSize) == false)
                    SetValue(ContentSizeProperty, value);
            }
        }

        private double _contentSize = 24;

        public static readonly DependencyProperty ContentSizeProperty =
            DependencyProperty.Register(nameof(ContentSize), typeof(double), typeof(BiaIconButton),
                new FrameworkPropertyMetadata(
                    Boxes.Double24,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaIconButton) s;
                        self._contentSize = (double) e.NewValue;
                    }));

        #endregion

        static BiaIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaIconButton),
                new FrameworkPropertyMetadata(typeof(BiaIconButton)));
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

            if (Content != null)
            {
                const double padding = 5;

                var size = Math.Max(Math.Min(ActualWidth, ActualHeight) - padding * 2, 0);
                _scale.ScaleX = size / ContentSize;
                _scale.ScaleY = size / ContentSize;
                _scale.CenterX = ActualWidth * 0.5;
                _scale.CenterY = ActualHeight * 0.5;

                dc.PushTransform(_scale);

                dc.DrawGeometry(Foreground, null, Content);

                dc.Pop();
            }
        }

        private readonly ScaleTransform _scale = new ScaleTransform();
   }
}