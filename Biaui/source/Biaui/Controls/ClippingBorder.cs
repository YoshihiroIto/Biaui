using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biaui.Controls
{
    // https://stackoverflow.com/questions/324641/how-to-make-the-contents-of-a-round-cornered-border-be-also-round-cornered
    public class ClippingBorder : Border
    {
        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        public override UIElement Child
        {
            get => base.Child;

            set
            {
                if (Child == value)
                    return;

                Child?.SetValue(ClipProperty, _oldClip);

                _oldClip = value?.ReadLocalValue(ClipProperty);

                base.Child = value;
            }
        }

        protected virtual void OnApplyChildClip()
        {
            var child = Child;

            if (child == null)
                return;

            var key = (Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5), new Rect(Child.RenderSize));

            if (_clipRectCache.TryGetValue(key, out var clipRect) == false)
            {
                var radius = Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5);

                clipRect = new RectangleGeometry
                {
                    RadiusX = radius,
                    RadiusY = radius,
                    Rect = new Rect(Child.RenderSize)
                };

                clipRect.Freeze();

                _clipRectCache.Add(key, clipRect);
            }

            child.Clip = clipRect;
        }

        private object _oldClip;

        private static readonly Dictionary<(double Radius, Rect Rect), RectangleGeometry> _clipRectCache =
            new Dictionary<(double Radius, Rect Rect), RectangleGeometry>();
    }
}