using System.Collections.Generic;
using System.Windows;
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

        public static Geometry GetClipGeom(double w, double h, double cornerRadius)
        {
            var key = (w, h, cornerRadius);
            if (_clipGeoms.TryGetValue(key, out var c))
                return c;

            c = new RectangleGeometry
            {
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                Rect = new Rect(0, 0, w, h)
            };

            c.Freeze();

            _clipGeoms.Add(key, c);

            return c;
        }

        private static readonly Dictionary<(double W, double H, double CorerRadius), RectangleGeometry> _clipGeoms =
            new Dictionary<(double W, double H, double CorerRadius), RectangleGeometry>();

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