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

            var rect = new Rect(Child.RenderSize);
            var key = (rect, CornerRadius);

            if (_clipRectCache.TryGetValue(key, out var clipRect) == false)
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                var isSame =
                    CornerRadius.TopLeft == CornerRadius.TopRight &&
                    CornerRadius.TopRight == CornerRadius.BottomRight &&
                    CornerRadius.BottomRight == CornerRadius.BottomLeft &&
                    CornerRadius.BottomLeft == CornerRadius.TopLeft;
                // ReSharper restore CompareOfFloatsByEqualityOperator

                clipRect = isSame
                    ? MakeRoundRectangleGeometrySameCorner(new Rect(Child.RenderSize), CornerRadius)
                    : MakeRoundRectangleGeometry(new Rect(Child.RenderSize), CornerRadius);

                _clipRectCache.Add(key, clipRect);
            }

            child.Clip = clipRect;
        }

        private object _oldClip;

        private static readonly Dictionary<(Rect Rect, CornerRadius CornerRadius), Geometry> _clipRectCache =
            new Dictionary<(Rect Rect, CornerRadius CornerRadius), Geometry>();

        private Geometry MakeRoundRectangleGeometrySameCorner(Rect baseRect, CornerRadius cornerRadius)
        {
            var radius = Math.Max(0.0, cornerRadius.TopLeft - BorderThickness.Left * 0.5);

            var clipRect = new RectangleGeometry
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = baseRect
            };

            clipRect.Freeze();

            return clipRect;
        }

        // https://wpfspark.wordpress.com/2011/06/04/handling-the-cornerradius-for-a-roundedrectangle-geometry-in-wpf/
        private static Geometry MakeRoundRectangleGeometry(Rect baseRect, CornerRadius cornerRadius)
        {
            if (cornerRadius.TopLeft < double.Epsilon)
                cornerRadius.TopLeft = 0.0;

            if (cornerRadius.TopRight < double.Epsilon)
                cornerRadius.TopRight = 0.0;

            if (cornerRadius.BottomLeft < double.Epsilon)
                cornerRadius.BottomLeft = 0.0;

            if (cornerRadius.BottomRight < double.Epsilon)
                cornerRadius.BottomRight = 0.0;

            var topLeftRect = new Rect(
                baseRect.Location.X,
                baseRect.Location.Y,
                cornerRadius.TopLeft,
                cornerRadius.TopLeft);

            var topRightRect = new Rect(
                baseRect.Location.X + baseRect.Width - cornerRadius.TopRight,
                baseRect.Location.Y,
                cornerRadius.TopRight,
                cornerRadius.TopRight);

            var bottomRightRect = new Rect(
                baseRect.Location.X + baseRect.Width - cornerRadius.BottomRight,
                baseRect.Location.Y + baseRect.Height - cornerRadius.BottomRight,
                cornerRadius.BottomRight,
                cornerRadius.BottomRight);

            var bottomLeftRect = new Rect(
                baseRect.Location.X,
                baseRect.Location.Y + baseRect.Height - cornerRadius.BottomLeft,
                cornerRadius.BottomLeft,
                cornerRadius.BottomLeft);

            if (topLeftRect.Right > topRightRect.Left)
            {
                var newWidth = topLeftRect.Width / (topLeftRect.Width + topRightRect.Width) * baseRect.Width;

                topLeftRect = new Rect(
                    topLeftRect.Location.X,
                    topLeftRect.Location.Y,
                    newWidth,
                    topLeftRect.Height);

                topRightRect = new Rect(
                    baseRect.Left + newWidth,
                    topRightRect.Location.Y,
                    Math.Max(0.0, baseRect.Width - newWidth),
                    topRightRect.Height);
            }

            if (topRightRect.Bottom > bottomRightRect.Top)
            {
                var newHeight = topRightRect.Height / (topRightRect.Height + bottomRightRect.Height) * baseRect.Height;

                topRightRect = new Rect(
                    topRightRect.Location.X,
                    topRightRect.Location.Y,
                    topRightRect.Width,
                    newHeight);

                bottomRightRect = new Rect(
                    bottomRightRect.Location.X,
                    baseRect.Top + newHeight,
                    bottomRightRect.Width,
                    Math.Max(0.0, baseRect.Height - newHeight));
            }

            if (bottomRightRect.Left < bottomLeftRect.Right)
            {
                var newWidth = bottomLeftRect.Width / (bottomLeftRect.Width + bottomRightRect.Width) * baseRect.Width;

                bottomLeftRect = new Rect(
                    bottomLeftRect.Location.X,
                    bottomLeftRect.Location.Y,
                    newWidth,
                    bottomLeftRect.Height);

                bottomRightRect = new Rect(
                    baseRect.Left + newWidth,
                    bottomRightRect.Location.Y,
                    Math.Max(0.0, baseRect.Width - newWidth),
                    bottomRightRect.Height);
            }

            if (bottomLeftRect.Top < topLeftRect.Bottom)
            {
                var newHeight = topLeftRect.Height / (topLeftRect.Height + bottomLeftRect.Height) * baseRect.Height;

                topLeftRect = new Rect(
                    topLeftRect.Location.X,
                    topLeftRect.Location.Y,
                    topLeftRect.Width,
                    newHeight);

                bottomLeftRect = new Rect(
                    bottomLeftRect.Location.X,
                    baseRect.Top + newHeight,
                    bottomLeftRect.Width,
                    Math.Max(0.0, baseRect.Height - newHeight));
            }

            var clipRect = new StreamGeometry();

            using (var context = clipRect.Open())
            {
                context.BeginFigure(topLeftRect.BottomLeft, true, true);

                context.ArcTo(topLeftRect.TopRight, topLeftRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(topRightRect.TopLeft, true, true);
                context.ArcTo(topRightRect.BottomRight, topRightRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(bottomRightRect.TopRight, true, true);
                context.ArcTo(bottomRightRect.BottomLeft, bottomRightRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.LineTo(bottomLeftRect.BottomRight, true, true);
                context.ArcTo(bottomLeftRect.TopLeft, bottomLeftRect.Size, 0, false, SweepDirection.Clockwise,
                    true, true);

                context.Close();
            }

            clipRect.Freeze();

            return clipRect;
        }
    }
}