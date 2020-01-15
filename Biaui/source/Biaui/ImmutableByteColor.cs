using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Biaui
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct ImmutableByteColor : IEquatable<ImmutableByteColor>
    {
        [FieldOffset(0)] public readonly byte A;
        [FieldOffset(1)] public readonly byte R;
        [FieldOffset(2)] public readonly byte G;
        [FieldOffset(3)] public readonly byte B;

        [FieldOffset(0)] private readonly int _argb;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableByteColor(byte a, byte r, byte g, byte b)
        {
            _argb = 0;
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public long HashCode => _argb;

        public override int GetHashCode()
        {
            return _argb;
        }

        public Color ToColor() => Color.FromArgb(A, R, G, B);

        public static bool operator ==(ImmutableByteColor color1, ImmutableByteColor color2) => color1._argb == color2._argb;
        public static bool operator !=(ImmutableByteColor color1, ImmutableByteColor color2) => color1._argb != color2._argb;

        public bool Equals(ImmutableByteColor other) => _argb == other._argb;
        public override bool Equals(object? obj) => obj is ImmutableByteColor other && Equals(other);

        private sealed class ImmutableByteColorEqualityComparer : IEqualityComparer<ImmutableByteColor>
        {
            public bool Equals(ImmutableByteColor x, ImmutableByteColor y) => x._argb == y._argb;
            public int GetHashCode(ImmutableByteColor obj) => obj._argb;
        }

        public static IEqualityComparer<ImmutableByteColor> ImmutableByteColorComparer { get; } = new ImmutableByteColorEqualityComparer();

        //
        public static readonly ImmutableByteColor Transparent = new ImmutableByteColor(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly ImmutableByteColor Black = new ImmutableByteColor(0xFF, 0x00, 0x00, 0x00);
        public static readonly ImmutableByteColor Navy = new ImmutableByteColor(0xFF, 0x00, 0x00, 0x80);
        public static readonly ImmutableByteColor DarkBlue = new ImmutableByteColor(0xFF, 0x00, 0x00, 0x8B);
        public static readonly ImmutableByteColor MediumBlue = new ImmutableByteColor(0xFF, 0x00, 0x00, 0xCD);
        public static readonly ImmutableByteColor Blue = new ImmutableByteColor(0xFF, 0x00, 0x00, 0xFF);
        public static readonly ImmutableByteColor DarkGreen = new ImmutableByteColor(0xFF, 0x00, 0x64, 0x00);
        public static readonly ImmutableByteColor Green = new ImmutableByteColor(0xFF, 0x00, 0x80, 0x00);
        public static readonly ImmutableByteColor Teal = new ImmutableByteColor(0xFF, 0x00, 0x80, 0x80);
        public static readonly ImmutableByteColor DarkCyan = new ImmutableByteColor(0xFF, 0x00, 0x8B, 0x8B);
        public static readonly ImmutableByteColor DeepSkyBlue = new ImmutableByteColor(0xFF, 0x00, 0xBF, 0xFF);
        public static readonly ImmutableByteColor DarkTurquoise = new ImmutableByteColor(0xFF, 0x00, 0xCE, 0xD1);
        public static readonly ImmutableByteColor MediumSpringGreen = new ImmutableByteColor(0xFF, 0x00, 0xFA, 0x9A);
        public static readonly ImmutableByteColor Lime = new ImmutableByteColor(0xFF, 0x00, 0xFF, 0x00);
        public static readonly ImmutableByteColor SpringGreen = new ImmutableByteColor(0xFF, 0x00, 0xFF, 0x7F);
        public static readonly ImmutableByteColor Aqua = new ImmutableByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly ImmutableByteColor Cyan = new ImmutableByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly ImmutableByteColor MidnightBlue = new ImmutableByteColor(0xFF, 0x19, 0x19, 0x70);
        public static readonly ImmutableByteColor DodgerBlue = new ImmutableByteColor(0xFF, 0x1E, 0x90, 0xFF);
        public static readonly ImmutableByteColor LightSeaGreen = new ImmutableByteColor(0xFF, 0x20, 0xB2, 0xAA);
        public static readonly ImmutableByteColor ForestGreen = new ImmutableByteColor(0xFF, 0x22, 0x8B, 0x22);
        public static readonly ImmutableByteColor SeaGreen = new ImmutableByteColor(0xFF, 0x2E, 0x8B, 0x57);
        public static readonly ImmutableByteColor DarkSlateGray = new ImmutableByteColor(0xFF, 0x2F, 0x4F, 0x4F);
        public static readonly ImmutableByteColor LimeGreen = new ImmutableByteColor(0xFF, 0x32, 0xCD, 0x32);
        public static readonly ImmutableByteColor MediumSeaGreen = new ImmutableByteColor(0xFF, 0x3C, 0xB3, 0x71);
        public static readonly ImmutableByteColor Turquoise = new ImmutableByteColor(0xFF, 0x40, 0xE0, 0xD0);
        public static readonly ImmutableByteColor RoyalBlue = new ImmutableByteColor(0xFF, 0x41, 0x69, 0xE1);
        public static readonly ImmutableByteColor SteelBlue = new ImmutableByteColor(0xFF, 0x46, 0x82, 0xB4);
        public static readonly ImmutableByteColor DarkSlateBlue = new ImmutableByteColor(0xFF, 0x48, 0x3D, 0x8B);
        public static readonly ImmutableByteColor MediumTurquoise = new ImmutableByteColor(0xFF, 0x48, 0xD1, 0xCC);
        public static readonly ImmutableByteColor Indigo = new ImmutableByteColor(0xFF, 0x4B, 0x00, 0x82);
        public static readonly ImmutableByteColor DarkOliveGreen = new ImmutableByteColor(0xFF, 0x55, 0x6B, 0x2F);
        public static readonly ImmutableByteColor CadetBlue = new ImmutableByteColor(0xFF, 0x5F, 0x9E, 0xA0);
        public static readonly ImmutableByteColor CornflowerBlue = new ImmutableByteColor(0xFF, 0x64, 0x95, 0xED);
        public static readonly ImmutableByteColor MediumAquamarine = new ImmutableByteColor(0xFF, 0x66, 0xCD, 0xAA);
        public static readonly ImmutableByteColor DimGray = new ImmutableByteColor(0xFF, 0x69, 0x69, 0x69);
        public static readonly ImmutableByteColor SlateBlue = new ImmutableByteColor(0xFF, 0x6A, 0x5A, 0xCD);
        public static readonly ImmutableByteColor OliveDrab = new ImmutableByteColor(0xFF, 0x6B, 0x8E, 0x23);
        public static readonly ImmutableByteColor SlateGray = new ImmutableByteColor(0xFF, 0x70, 0x80, 0x90);
        public static readonly ImmutableByteColor LightSlateGray = new ImmutableByteColor(0xFF, 0x77, 0x88, 0x99);
        public static readonly ImmutableByteColor MediumSlateBlue = new ImmutableByteColor(0xFF, 0x7B, 0x68, 0xEE);
        public static readonly ImmutableByteColor LawnGreen = new ImmutableByteColor(0xFF, 0x7C, 0xFC, 0x00);
        public static readonly ImmutableByteColor Chartreuse = new ImmutableByteColor(0xFF, 0x7F, 0xFF, 0x00);
        public static readonly ImmutableByteColor Aquamarine = new ImmutableByteColor(0xFF, 0x7F, 0xFF, 0xD4);
        public static readonly ImmutableByteColor Maroon = new ImmutableByteColor(0xFF, 0x80, 0x00, 0x00);
        public static readonly ImmutableByteColor Purple = new ImmutableByteColor(0xFF, 0x80, 0x00, 0x80);
        public static readonly ImmutableByteColor Olive = new ImmutableByteColor(0xFF, 0x80, 0x80, 0x00);
        public static readonly ImmutableByteColor Gray = new ImmutableByteColor(0xFF, 0x80, 0x80, 0x80);
        public static readonly ImmutableByteColor SkyBlue = new ImmutableByteColor(0xFF, 0x87, 0xCE, 0xEB);
        public static readonly ImmutableByteColor LightSkyBlue = new ImmutableByteColor(0xFF, 0x87, 0xCE, 0xFA);
        public static readonly ImmutableByteColor BlueViolet = new ImmutableByteColor(0xFF, 0x8A, 0x2B, 0xE2);
        public static readonly ImmutableByteColor DarkRed = new ImmutableByteColor(0xFF, 0x8B, 0x00, 0x00);
        public static readonly ImmutableByteColor DarkMagenta = new ImmutableByteColor(0xFF, 0x8B, 0x00, 0x8B);
        public static readonly ImmutableByteColor SaddleBrown = new ImmutableByteColor(0xFF, 0x8B, 0x45, 0x13);
        public static readonly ImmutableByteColor DarkSeaGreen = new ImmutableByteColor(0xFF, 0x8F, 0xBC, 0x8F);
        public static readonly ImmutableByteColor LightGreen = new ImmutableByteColor(0xFF, 0x90, 0xEE, 0x90);
        public static readonly ImmutableByteColor MediumPurple = new ImmutableByteColor(0xFF, 0x93, 0x70, 0xDB);
        public static readonly ImmutableByteColor DarkViolet = new ImmutableByteColor(0xFF, 0x94, 0x00, 0xD3);
        public static readonly ImmutableByteColor PaleGreen = new ImmutableByteColor(0xFF, 0x98, 0xFB, 0x98);
        public static readonly ImmutableByteColor DarkOrchid = new ImmutableByteColor(0xFF, 0x99, 0x32, 0xCC);
        public static readonly ImmutableByteColor YellowGreen = new ImmutableByteColor(0xFF, 0x9A, 0xCD, 0x32);
        public static readonly ImmutableByteColor Sienna = new ImmutableByteColor(0xFF, 0xA0, 0x52, 0x2D);
        public static readonly ImmutableByteColor Brown = new ImmutableByteColor(0xFF, 0xA5, 0x2A, 0x2A);
        public static readonly ImmutableByteColor DarkGray = new ImmutableByteColor(0xFF, 0xA9, 0xA9, 0xA9);
        public static readonly ImmutableByteColor LightBlue = new ImmutableByteColor(0xFF, 0xAD, 0xD8, 0xE6);
        public static readonly ImmutableByteColor GreenYellow = new ImmutableByteColor(0xFF, 0xAD, 0xFF, 0x2F);
        public static readonly ImmutableByteColor PaleTurquoise = new ImmutableByteColor(0xFF, 0xAF, 0xEE, 0xEE);
        public static readonly ImmutableByteColor LightSteelBlue = new ImmutableByteColor(0xFF, 0xB0, 0xC4, 0xDE);
        public static readonly ImmutableByteColor PowderBlue = new ImmutableByteColor(0xFF, 0xB0, 0xE0, 0xE6);
        public static readonly ImmutableByteColor Firebrick = new ImmutableByteColor(0xFF, 0xB2, 0x22, 0x22);
        public static readonly ImmutableByteColor DarkGoldenrod = new ImmutableByteColor(0xFF, 0xB8, 0x86, 0x0B);
        public static readonly ImmutableByteColor MediumOrchid = new ImmutableByteColor(0xFF, 0xBA, 0x55, 0xD3);
        public static readonly ImmutableByteColor RosyBrown = new ImmutableByteColor(0xFF, 0xBC, 0x8F, 0x8F);
        public static readonly ImmutableByteColor DarkKhaki = new ImmutableByteColor(0xFF, 0xBD, 0xB7, 0x6B);
        public static readonly ImmutableByteColor Silver = new ImmutableByteColor(0xFF, 0xC0, 0xC0, 0xC0);
        public static readonly ImmutableByteColor MediumVioletRed = new ImmutableByteColor(0xFF, 0xC7, 0x15, 0x85);
        public static readonly ImmutableByteColor IndianRed = new ImmutableByteColor(0xFF, 0xCD, 0x5C, 0x5C);
        public static readonly ImmutableByteColor Peru = new ImmutableByteColor(0xFF, 0xCD, 0x85, 0x3F);
        public static readonly ImmutableByteColor Chocolate = new ImmutableByteColor(0xFF, 0xD2, 0x69, 0x1E);
        public static readonly ImmutableByteColor Tan = new ImmutableByteColor(0xFF, 0xD2, 0xB4, 0x8C);
        public static readonly ImmutableByteColor LightGray = new ImmutableByteColor(0xFF, 0xD3, 0xD3, 0xD3);
        public static readonly ImmutableByteColor Thistle = new ImmutableByteColor(0xFF, 0xD8, 0xBF, 0xD8);
        public static readonly ImmutableByteColor Orchid = new ImmutableByteColor(0xFF, 0xDA, 0x70, 0xD6);
        public static readonly ImmutableByteColor Goldenrod = new ImmutableByteColor(0xFF, 0xDA, 0xA5, 0x20);
        public static readonly ImmutableByteColor PaleVioletRed = new ImmutableByteColor(0xFF, 0xDB, 0x70, 0x93);
        public static readonly ImmutableByteColor Crimson = new ImmutableByteColor(0xFF, 0xDC, 0x14, 0x3C);
        public static readonly ImmutableByteColor Gainsboro = new ImmutableByteColor(0xFF, 0xDC, 0xDC, 0xDC);
        public static readonly ImmutableByteColor Plum = new ImmutableByteColor(0xFF, 0xDD, 0xA0, 0xDD);
        public static readonly ImmutableByteColor BurlyWood = new ImmutableByteColor(0xFF, 0xDE, 0xB8, 0x87);
        public static readonly ImmutableByteColor LightCyan = new ImmutableByteColor(0xFF, 0xE0, 0xFF, 0xFF);
        public static readonly ImmutableByteColor Lavender = new ImmutableByteColor(0xFF, 0xE6, 0xE6, 0xFA);
        public static readonly ImmutableByteColor DarkSalmon = new ImmutableByteColor(0xFF, 0xE9, 0x96, 0x7A);
        public static readonly ImmutableByteColor Violet = new ImmutableByteColor(0xFF, 0xEE, 0x82, 0xEE);
        public static readonly ImmutableByteColor PaleGoldenrod = new ImmutableByteColor(0xFF, 0xEE, 0xE8, 0xAA);
        public static readonly ImmutableByteColor LightCoral = new ImmutableByteColor(0xFF, 0xF0, 0x80, 0x80);
        public static readonly ImmutableByteColor Khaki = new ImmutableByteColor(0xFF, 0xF0, 0xE6, 0x8C);
        public static readonly ImmutableByteColor AliceBlue = new ImmutableByteColor(0xFF, 0xF0, 0xF8, 0xFF);
        public static readonly ImmutableByteColor Honeydew = new ImmutableByteColor(0xFF, 0xF0, 0xFF, 0xF0);
        public static readonly ImmutableByteColor Azure = new ImmutableByteColor(0xFF, 0xF0, 0xFF, 0xFF);
        public static readonly ImmutableByteColor SandyBrown = new ImmutableByteColor(0xFF, 0xF4, 0xA4, 0x60);
        public static readonly ImmutableByteColor Wheat = new ImmutableByteColor(0xFF, 0xF5, 0xDE, 0xB3);
        public static readonly ImmutableByteColor Beige = new ImmutableByteColor(0xFF, 0xF5, 0xF5, 0xDC);
        public static readonly ImmutableByteColor WhiteSmoke = new ImmutableByteColor(0xFF, 0xF5, 0xF5, 0xF5);
        public static readonly ImmutableByteColor MintCream = new ImmutableByteColor(0xFF, 0xF5, 0xFF, 0xFA);
        public static readonly ImmutableByteColor GhostWhite = new ImmutableByteColor(0xFF, 0xF8, 0xF8, 0xFF);
        public static readonly ImmutableByteColor Salmon = new ImmutableByteColor(0xFF, 0xFA, 0x80, 0x72);
        public static readonly ImmutableByteColor AntiqueWhite = new ImmutableByteColor(0xFF, 0xFA, 0xEB, 0xD7);
        public static readonly ImmutableByteColor Linen = new ImmutableByteColor(0xFF, 0xFA, 0xF0, 0xE6);
        public static readonly ImmutableByteColor LightGoldenrodYellow = new ImmutableByteColor(0xFF, 0xFA, 0xFA, 0xD2);
        public static readonly ImmutableByteColor OldLace = new ImmutableByteColor(0xFF, 0xFD, 0xF5, 0xE6);
        public static readonly ImmutableByteColor Red = new ImmutableByteColor(0xFF, 0xFF, 0x00, 0x00);
        public static readonly ImmutableByteColor Fuchsia = new ImmutableByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static readonly ImmutableByteColor Magenta = new ImmutableByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static readonly ImmutableByteColor DeepPink = new ImmutableByteColor(0xFF, 0xFF, 0x14, 0x93);
        public static readonly ImmutableByteColor OrangeRed = new ImmutableByteColor(0xFF, 0xFF, 0x45, 0x00);
        public static readonly ImmutableByteColor Tomato = new ImmutableByteColor(0xFF, 0xFF, 0x63, 0x47);
        public static readonly ImmutableByteColor HotPink = new ImmutableByteColor(0xFF, 0xFF, 0x69, 0xB4);
        public static readonly ImmutableByteColor Coral = new ImmutableByteColor(0xFF, 0xFF, 0x7F, 0x50);
        public static readonly ImmutableByteColor DarkOrange = new ImmutableByteColor(0xFF, 0xFF, 0x8C, 0x00);
        public static readonly ImmutableByteColor LightSalmon = new ImmutableByteColor(0xFF, 0xFF, 0xA0, 0x7A);
        public static readonly ImmutableByteColor Orange = new ImmutableByteColor(0xFF, 0xFF, 0xA5, 0x00);
        public static readonly ImmutableByteColor LightPink = new ImmutableByteColor(0xFF, 0xFF, 0xB6, 0xC1);
        public static readonly ImmutableByteColor Pink = new ImmutableByteColor(0xFF, 0xFF, 0xC0, 0xCB);
        public static readonly ImmutableByteColor Gold = new ImmutableByteColor(0xFF, 0xFF, 0xD7, 0x00);
        public static readonly ImmutableByteColor PeachPuff = new ImmutableByteColor(0xFF, 0xFF, 0xDA, 0xB9);
        public static readonly ImmutableByteColor NavajoWhite = new ImmutableByteColor(0xFF, 0xFF, 0xDE, 0xAD);
        public static readonly ImmutableByteColor Moccasin = new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xB5);
        public static readonly ImmutableByteColor Bisque = new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xC4);
        public static readonly ImmutableByteColor MistyRose = new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xE1);
        public static readonly ImmutableByteColor BlanchedAlmond = new ImmutableByteColor(0xFF, 0xFF, 0xEB, 0xCD);
        public static readonly ImmutableByteColor PapayaWhip = new ImmutableByteColor(0xFF, 0xFF, 0xEF, 0xD5);
        public static readonly ImmutableByteColor LavenderBlush = new ImmutableByteColor(0xFF, 0xFF, 0xF0, 0xF5);
        public static readonly ImmutableByteColor SeaShell = new ImmutableByteColor(0xFF, 0xFF, 0xF5, 0xEE);
        public static readonly ImmutableByteColor Cornsilk = new ImmutableByteColor(0xFF, 0xFF, 0xF8, 0xDC);
        public static readonly ImmutableByteColor LemonChiffon = new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xCD);
        public static readonly ImmutableByteColor FloralWhite = new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xF0);
        public static readonly ImmutableByteColor Snow = new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xFA);
        public static readonly ImmutableByteColor Yellow = new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0x00);
        public static readonly ImmutableByteColor LightYellow = new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xE0);
        public static readonly ImmutableByteColor Ivory = new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xF0);
        public static readonly ImmutableByteColor White = new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xFF);
    }

    public static class ColorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableByteColor ToImmutableByteColor(this Color src)
            => new ImmutableByteColor(src.A, src.R, src.G, src.B);
    }
}