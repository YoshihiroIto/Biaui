using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class Caches
    {
        internal static Pen GetBorderPen(Color color, double thickness)
        {
            var key = (color, thickness);

            if (_borderPens.TryGetValue(key, out var p))
                return p;

            p = new Pen(GetSolidColorBrush(color), thickness);
            p.Freeze();

            _borderPens.Add(key, p);

            return p;
        }

        internal static SolidColorBrush GetSolidColorBrush(Color color)
        {
            if (_solidColorBrushes.TryGetValue(color, out var b))
                return b;

            b = new SolidColorBrush(color);
            b.Freeze();

            _solidColorBrushes.Add(color, b);

            return b;
        }

        internal static Geometry GetClipGeom(double w, double h, double cornerRadius, bool isWidthBorder)
        {
            var key = (w, h, cornerRadius, isWidthBorder);
            if (_clipGeoms.TryGetValue(key, out var c))
                return c;

            if (isWidthBorder)
            {
                c = new RectangleGeometry
                {
                    RadiusX = cornerRadius,
                    RadiusY = cornerRadius,
                    Rect = FrameworkElementHelper.RoundLayoutRect(
                        FrameworkElementExtensions.BorderWidth * 0.5,
                        FrameworkElementExtensions.BorderWidth * 0.5,
                        w - FrameworkElementExtensions.BorderWidth,
                        h - FrameworkElementExtensions.BorderWidth)
                };
            }
            else
            {
                c = new RectangleGeometry
                {
                    RadiusX = cornerRadius,
                    RadiusY = cornerRadius,
                    Rect = FrameworkElementHelper.RoundLayoutRect(0, 0, w, h)
                };
            }

            c.Freeze();

            _clipGeoms.Add(key, c);

            return c;
        }

        private static readonly
            Dictionary<(double W, double H, double CorerRadius, bool IsWidthBorder), RectangleGeometry> _clipGeoms =
                new Dictionary<(double W, double H, double CorerRadius, bool IsWidthBorder), RectangleGeometry>();

        private static readonly Dictionary<(Color, double), Pen> _borderPens = new Dictionary<(Color, double), Pen>();

        private static readonly Dictionary<Color, SolidColorBrush> _solidColorBrushes =
            new Dictionary<Color, SolidColorBrush>();


        internal static TraversalRequest PreviousTraversalRequest
            => _PreviousTraversalRequest ??
               (_PreviousTraversalRequest = new TraversalRequest(FocusNavigationDirection.Previous));

        internal static TraversalRequest NextTraversalRequest
            => _NextTraversalRequest ??
               (_NextTraversalRequest = new TraversalRequest(FocusNavigationDirection.Next));

        private static TraversalRequest _PreviousTraversalRequest;
        private static TraversalRequest _NextTraversalRequest;
    }
}