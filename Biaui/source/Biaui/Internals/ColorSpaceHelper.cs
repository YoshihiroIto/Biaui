using System;

namespace Biaui.Internals;

internal static class ColorSpaceHelper
{
    internal static (double Hue, double Saturation, double Value) RgbToHsv(double red, double green, double blue)
    {
        var max = (red, green, blue).Max();
        var min = (red, green, blue).Min();

        var h = 0d;
        var s = 0d;
        var v = max;

        if (NumberHelper.AreClose(max, min) == false)
        {
            if (NumberHelper.AreClose(max, red)) h = 60d / 360d * (green - blue) / (max - min);
            else if (NumberHelper.AreClose(max, green)) h = 60d / 360d * (blue - red) / (max - min) + 120d / 360d;
            else if (NumberHelper.AreClose(max, blue)) h = 60d / 360d * (red - green) / (max - min) + 240d / 360d;

            s = (max - min) / max;
        }

        if (h < 0d)
            h += 1d;

        return (h, s, v);
    }

    internal static (double Red, double Green, double Blue) HsvToRgb(double hue, double saturation, double value)
    {
        var h = NumberHelper.AreClose(hue, 1d) ? 0d : hue;
        var s = saturation;
        var v = value;

        if (NumberHelper.AreCloseZero(s))
            return (v, v, v);

        var dh = Math.Floor(h / (60d / 360d));
        var p = v * (1d - s);
        var q = v * (1d - s * (h / (60d / 360d) - dh));
        var t = v * (1d - s * (1d - (h / (60d / 360d) - dh)));

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