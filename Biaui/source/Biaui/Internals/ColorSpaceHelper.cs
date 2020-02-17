using System;

namespace Biaui.Internals
{
    internal static class ColorSpaceHelper
    {
        internal static (double Hue, double Saturation, double Value) RgbToHsv(double red, double green, double blue)
        {
            var max = (red, green, blue).Max();
            var min = (red, green, blue).Min();

            var h = 0.0;
            var s = 0.0;
            var v = max;

            if (NumberHelper.AreClose(max, min) == false)
            {
                if (NumberHelper.AreClose(max, red)) h = 60.0 / 360 * (green - blue) / (max - min);
                else if (NumberHelper.AreClose(max, green)) h = 60.0 / 360 * (blue - red) / (max - min) + 120.0 / 360;
                else if (NumberHelper.AreClose(max, blue)) h = 60.0 / 360 * (red - green) / (max - min) + 240.0 / 360;

                s = (max - min) / max;
            }

            if (h < 0)
                h += 1;

            return (h, s, v);
        }

        internal static (double Red, double Green, double Blue) HsvToRgb(double hue, double saturation, double value)
        {
            var h = NumberHelper.AreClose(hue, 1) ? 0 : hue;
            var s = saturation;
            var v = value;

            if (NumberHelper.AreCloseZero(s))
                return (v, v, v);

            var dh = Math.Floor(h / (60.0 / 360));
            var p = v * (1 - s);
            var q = v * (1 - s * (h / (60.0 / 360) - dh));
            var t = v * (1 - s * (1 - (h / (60.0 / 360) - dh)));

            return dh switch
            {
                0 => (v, t, p),
                1 => (q, v, p),
                2 => (p, v, t),
                3 => (p, q, v),
                4 => (t, p, v),
                5 => (v, p, q),
                _ => throw new Exception()
            };
        }
    }
}