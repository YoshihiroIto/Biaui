using System;

namespace Biaui.Internals
{
    internal static class ColorSpaceHelper
    {
        internal static (double Hue, double Saturation, double Value) RgbToHsv(double red, double green, double blue)
        {
            var max = NumberHelper.Max(NumberHelper.Max(red, green), blue);
            var min = NumberHelper.Min(NumberHelper.Min(red, green), blue);

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
                h = h + 1;

            return (h, s, v);
        }

        internal static (double Red, double Green, double Blue) HsvToRgb(double hue, double saturation, double value)
        {
            var h = NumberHelper.AreClose(hue, 1) ? 0 : hue;
            var s = saturation;
            var v = value;

            double r, g, b;

            if (NumberHelper.AreCloseZero(s))
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                var dh = Math.Floor(h / (60.0 / 360));
                var p = v * (1 - s);
                var q = v * (1 - s * (h / (60.0 / 360) - dh));
                var t = v * (1 - s * (1 - (h / (60.0 / 360) - dh)));

                switch (dh)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r   = p;
                        g = q;
                        b  = v;
                        break;

                    case 4:
                        r   = t;
                        g = p;
                        b  = v;
                        break;

                    case 5:
                        r   = v;
                        g = p;
                        b  = q;
                        break;

                    default:
                        throw new Exception();
                }
            }
            
            return (r, g, b);
        }
    }
}