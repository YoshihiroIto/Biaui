using System.Collections.Generic;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class Caches
    {
        public static readonly Pen PointIn;
        public static readonly Pen PointInIsReadOnly;
        public static readonly Pen PointOut;

        public static Pen GetBorderPen(Color color, double thickness)
        {
            var key = (color, thickness);

            if (_borderPens.TryGetValue(key, out var p))
                return p;

            var b = new SolidColorBrush(color);
            b.Freeze();

            p = new Pen(b, thickness);
            p.Freeze();

            _borderPens.Add(key, p);

            return p;
        }

        public static Geometry GetClipGeom(double w, double h, double cornerRadius, bool isWidthBorder)
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

        private static readonly Dictionary<(double W, double H, double CorerRadius, bool IsWidthBorder), RectangleGeometry> _clipGeoms =
            new Dictionary<(double W, double H, double CorerRadius, bool IsWidthBorder), RectangleGeometry>();

        private static readonly Dictionary<(Color, double), Pen> _borderPens = new Dictionary<(Color, double), Pen>();

        static Caches()
        {
            PointIn = new Pen(Brushes.White, FrameworkElementHelper.RoundLayoutValue(2));
            PointIn.Freeze();

            PointInIsReadOnly = new Pen(Brushes.Gray, FrameworkElementHelper.RoundLayoutValue(2));
            PointInIsReadOnly.Freeze();

            PointOut = new Pen(Brushes.Black, FrameworkElementHelper.RoundLayoutValue(4));
            PointOut.Freeze();
        }
    }
}