using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;
using Jewelry.Collections;

namespace Biaui.Controls
{
    public class BiaIconButton : BiaButtonBase
    {
        #region Content

        public Geometry? Content
        {
            get => _content;
            set
            {
                if (value != _content)
                    SetValue(ContentProperty, value);
            }
        }

        private Geometry? _content;

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
                    SetValue(ContentSizeProperty, Boxes.Double(value));
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
                if (double.IsNaN(ContentSize) ||
                    NumberHelper.AreCloseZero(ContentSize))
                {
                    dc.DrawGeometry(Foreground, null, Content);
                }
                else
                {
                    dc.PushTransform(GetScale());

                    dc.DrawGeometry(Foreground, null, Content);

                    dc.Pop();
                }
            }
        }

        private ScaleTransform GetScale()
        {
            const double padding = 5;

            var hash = HashCodeMaker.Make(ActualWidth, ActualHeight, ContentSize);

            if (_scaleCache.TryGetValue(hash, out var scale) == false)
            {
                scale = new ScaleTransform();

                var size = Math.Max(Math.Min(ActualWidth, ActualHeight) - padding * 2, 0);
                scale.ScaleX = size / ContentSize;
                scale.ScaleY = size / ContentSize;
                scale.CenterX = ActualWidth * 0.5;
                scale.CenterY = ActualHeight * 0.5;

                _scaleCache.Add(hash, scale);
            }

            return scale;
        }

        private static readonly LruCache<long, ScaleTransform> _scaleCache = new LruCache<long, ScaleTransform>(16);
    }
}