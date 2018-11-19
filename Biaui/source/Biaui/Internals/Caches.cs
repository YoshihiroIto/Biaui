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

        public static GuidelineSet GetGuidelineSet(Rect rect, double borderWidth)
        {
            var halfPenWidth = borderWidth / WpfHelper.PixelsPerDip / 2;

            var key = (rect, halfPenWidth);

            if (_guidelineSets.TryGetValue(key, out var p))
                return p;

            var guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
            guidelines.Freeze();

            _guidelineSets.Add(key, guidelines);

            return guidelines;
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

        private static readonly Dictionary<(Rect, double), GuidelineSet> _guidelineSets =
            new Dictionary<(Rect, double), GuidelineSet>();

        static Caches()
        {
            PointIn = new Pen(Brushes.White, 2 / WpfHelper.PixelsPerDip);
            PointIn.Freeze();

            PointInIsReadOnly = new Pen(Brushes.Gray, 2 / WpfHelper.PixelsPerDip);
            PointInIsReadOnly.Freeze();

            PointOut = new Pen(Brushes.Black, 5 / WpfHelper.PixelsPerDip);
            PointOut.Freeze();
        }
    }
}