using System;
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
            
            _clipRect.RadiusX = _clipRect.RadiusY = Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5);
            _clipRect.Rect = new Rect(Child.RenderSize);
            child.Clip = _clipRect;
        }

        private readonly RectangleGeometry _clipRect = new RectangleGeometry();
        private object _oldClip;
    }
}