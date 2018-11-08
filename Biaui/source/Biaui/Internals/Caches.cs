using System.Collections.Generic;
using System.Windows.Media;

namespace Biaui.Internals
{
    public static class Caches
    {
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

        private static readonly Dictionary<(Color, double), Pen> _borderPens = new Dictionary<(Color, double), Pen>();
    }
}