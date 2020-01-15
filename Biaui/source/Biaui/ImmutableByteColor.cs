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
        public static ImmutableByteColor Transparent          => new ImmutableByteColor(0x00, 0xFF, 0xFF, 0xFF);
        public static ImmutableByteColor Black                => new ImmutableByteColor(0xFF, 0x00, 0x00, 0x00);
        public static ImmutableByteColor Navy                 => new ImmutableByteColor(0xFF, 0x00, 0x00, 0x80);
        public static ImmutableByteColor DarkBlue             => new ImmutableByteColor(0xFF, 0x00, 0x00, 0x8B);
        public static ImmutableByteColor MediumBlue           => new ImmutableByteColor(0xFF, 0x00, 0x00, 0xCD);
        public static ImmutableByteColor Blue                 => new ImmutableByteColor(0xFF, 0x00, 0x00, 0xFF);
        public static ImmutableByteColor DarkGreen            => new ImmutableByteColor(0xFF, 0x00, 0x64, 0x00);
        public static ImmutableByteColor Green                => new ImmutableByteColor(0xFF, 0x00, 0x80, 0x00);
        public static ImmutableByteColor Teal                 => new ImmutableByteColor(0xFF, 0x00, 0x80, 0x80);
        public static ImmutableByteColor DarkCyan             => new ImmutableByteColor(0xFF, 0x00, 0x8B, 0x8B);
        public static ImmutableByteColor DeepSkyBlue          => new ImmutableByteColor(0xFF, 0x00, 0xBF, 0xFF);
        public static ImmutableByteColor DarkTurquoise        => new ImmutableByteColor(0xFF, 0x00, 0xCE, 0xD1);
        public static ImmutableByteColor MediumSpringGreen    => new ImmutableByteColor(0xFF, 0x00, 0xFA, 0x9A);
        public static ImmutableByteColor Lime                 => new ImmutableByteColor(0xFF, 0x00, 0xFF, 0x00);
        public static ImmutableByteColor SpringGreen          => new ImmutableByteColor(0xFF, 0x00, 0xFF, 0x7F);
        public static ImmutableByteColor Aqua                 => new ImmutableByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static ImmutableByteColor Cyan                 => new ImmutableByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static ImmutableByteColor MidnightBlue         => new ImmutableByteColor(0xFF, 0x19, 0x19, 0x70);
        public static ImmutableByteColor DodgerBlue           => new ImmutableByteColor(0xFF, 0x1E, 0x90, 0xFF);
        public static ImmutableByteColor LightSeaGreen        => new ImmutableByteColor(0xFF, 0x20, 0xB2, 0xAA);
        public static ImmutableByteColor ForestGreen          => new ImmutableByteColor(0xFF, 0x22, 0x8B, 0x22);
        public static ImmutableByteColor SeaGreen             => new ImmutableByteColor(0xFF, 0x2E, 0x8B, 0x57);
        public static ImmutableByteColor DarkSlateGray        => new ImmutableByteColor(0xFF, 0x2F, 0x4F, 0x4F);
        public static ImmutableByteColor LimeGreen            => new ImmutableByteColor(0xFF, 0x32, 0xCD, 0x32);
        public static ImmutableByteColor MediumSeaGreen       => new ImmutableByteColor(0xFF, 0x3C, 0xB3, 0x71);
        public static ImmutableByteColor Turquoise            => new ImmutableByteColor(0xFF, 0x40, 0xE0, 0xD0);
        public static ImmutableByteColor RoyalBlue            => new ImmutableByteColor(0xFF, 0x41, 0x69, 0xE1);
        public static ImmutableByteColor SteelBlue            => new ImmutableByteColor(0xFF, 0x46, 0x82, 0xB4);
        public static ImmutableByteColor DarkSlateBlue        => new ImmutableByteColor(0xFF, 0x48, 0x3D, 0x8B);
        public static ImmutableByteColor MediumTurquoise      => new ImmutableByteColor(0xFF, 0x48, 0xD1, 0xCC);
        public static ImmutableByteColor Indigo               => new ImmutableByteColor(0xFF, 0x4B, 0x00, 0x82);
        public static ImmutableByteColor DarkOliveGreen       => new ImmutableByteColor(0xFF, 0x55, 0x6B, 0x2F);
        public static ImmutableByteColor CadetBlue            => new ImmutableByteColor(0xFF, 0x5F, 0x9E, 0xA0);
        public static ImmutableByteColor CornflowerBlue       => new ImmutableByteColor(0xFF, 0x64, 0x95, 0xED);
        public static ImmutableByteColor MediumAquamarine     => new ImmutableByteColor(0xFF, 0x66, 0xCD, 0xAA);
        public static ImmutableByteColor DimGray              => new ImmutableByteColor(0xFF, 0x69, 0x69, 0x69);
        public static ImmutableByteColor SlateBlue            => new ImmutableByteColor(0xFF, 0x6A, 0x5A, 0xCD);
        public static ImmutableByteColor OliveDrab            => new ImmutableByteColor(0xFF, 0x6B, 0x8E, 0x23);
        public static ImmutableByteColor SlateGray            => new ImmutableByteColor(0xFF, 0x70, 0x80, 0x90);
        public static ImmutableByteColor LightSlateGray       => new ImmutableByteColor(0xFF, 0x77, 0x88, 0x99);
        public static ImmutableByteColor MediumSlateBlue      => new ImmutableByteColor(0xFF, 0x7B, 0x68, 0xEE);
        public static ImmutableByteColor LawnGreen            => new ImmutableByteColor(0xFF, 0x7C, 0xFC, 0x00);
        public static ImmutableByteColor Chartreuse           => new ImmutableByteColor(0xFF, 0x7F, 0xFF, 0x00);
        public static ImmutableByteColor Aquamarine           => new ImmutableByteColor(0xFF, 0x7F, 0xFF, 0xD4);
        public static ImmutableByteColor Maroon               => new ImmutableByteColor(0xFF, 0x80, 0x00, 0x00);
        public static ImmutableByteColor Purple               => new ImmutableByteColor(0xFF, 0x80, 0x00, 0x80);
        public static ImmutableByteColor Olive                => new ImmutableByteColor(0xFF, 0x80, 0x80, 0x00);
        public static ImmutableByteColor Gray                 => new ImmutableByteColor(0xFF, 0x80, 0x80, 0x80);
        public static ImmutableByteColor SkyBlue              => new ImmutableByteColor(0xFF, 0x87, 0xCE, 0xEB);
        public static ImmutableByteColor LightSkyBlue         => new ImmutableByteColor(0xFF, 0x87, 0xCE, 0xFA);
        public static ImmutableByteColor BlueViolet           => new ImmutableByteColor(0xFF, 0x8A, 0x2B, 0xE2);
        public static ImmutableByteColor DarkRed              => new ImmutableByteColor(0xFF, 0x8B, 0x00, 0x00);
        public static ImmutableByteColor DarkMagenta          => new ImmutableByteColor(0xFF, 0x8B, 0x00, 0x8B);
        public static ImmutableByteColor SaddleBrown          => new ImmutableByteColor(0xFF, 0x8B, 0x45, 0x13);
        public static ImmutableByteColor DarkSeaGreen         => new ImmutableByteColor(0xFF, 0x8F, 0xBC, 0x8F);
        public static ImmutableByteColor LightGreen           => new ImmutableByteColor(0xFF, 0x90, 0xEE, 0x90);
        public static ImmutableByteColor MediumPurple         => new ImmutableByteColor(0xFF, 0x93, 0x70, 0xDB);
        public static ImmutableByteColor DarkViolet           => new ImmutableByteColor(0xFF, 0x94, 0x00, 0xD3);
        public static ImmutableByteColor PaleGreen            => new ImmutableByteColor(0xFF, 0x98, 0xFB, 0x98);
        public static ImmutableByteColor DarkOrchid           => new ImmutableByteColor(0xFF, 0x99, 0x32, 0xCC);
        public static ImmutableByteColor YellowGreen          => new ImmutableByteColor(0xFF, 0x9A, 0xCD, 0x32);
        public static ImmutableByteColor Sienna               => new ImmutableByteColor(0xFF, 0xA0, 0x52, 0x2D);
        public static ImmutableByteColor Brown                => new ImmutableByteColor(0xFF, 0xA5, 0x2A, 0x2A);
        public static ImmutableByteColor DarkGray             => new ImmutableByteColor(0xFF, 0xA9, 0xA9, 0xA9);
        public static ImmutableByteColor LightBlue            => new ImmutableByteColor(0xFF, 0xAD, 0xD8, 0xE6);
        public static ImmutableByteColor GreenYellow          => new ImmutableByteColor(0xFF, 0xAD, 0xFF, 0x2F);
        public static ImmutableByteColor PaleTurquoise        => new ImmutableByteColor(0xFF, 0xAF, 0xEE, 0xEE);
        public static ImmutableByteColor LightSteelBlue       => new ImmutableByteColor(0xFF, 0xB0, 0xC4, 0xDE);
        public static ImmutableByteColor PowderBlue           => new ImmutableByteColor(0xFF, 0xB0, 0xE0, 0xE6);
        public static ImmutableByteColor Firebrick            => new ImmutableByteColor(0xFF, 0xB2, 0x22, 0x22);
        public static ImmutableByteColor DarkGoldenrod        => new ImmutableByteColor(0xFF, 0xB8, 0x86, 0x0B);
        public static ImmutableByteColor MediumOrchid         => new ImmutableByteColor(0xFF, 0xBA, 0x55, 0xD3);
        public static ImmutableByteColor RosyBrown            => new ImmutableByteColor(0xFF, 0xBC, 0x8F, 0x8F);
        public static ImmutableByteColor DarkKhaki            => new ImmutableByteColor(0xFF, 0xBD, 0xB7, 0x6B);
        public static ImmutableByteColor Silver               => new ImmutableByteColor(0xFF, 0xC0, 0xC0, 0xC0);
        public static ImmutableByteColor MediumVioletRed      => new ImmutableByteColor(0xFF, 0xC7, 0x15, 0x85);
        public static ImmutableByteColor IndianRed            => new ImmutableByteColor(0xFF, 0xCD, 0x5C, 0x5C);
        public static ImmutableByteColor Peru                 => new ImmutableByteColor(0xFF, 0xCD, 0x85, 0x3F);
        public static ImmutableByteColor Chocolate            => new ImmutableByteColor(0xFF, 0xD2, 0x69, 0x1E);
        public static ImmutableByteColor Tan                  => new ImmutableByteColor(0xFF, 0xD2, 0xB4, 0x8C);
        public static ImmutableByteColor LightGray            => new ImmutableByteColor(0xFF, 0xD3, 0xD3, 0xD3);
        public static ImmutableByteColor Thistle              => new ImmutableByteColor(0xFF, 0xD8, 0xBF, 0xD8);
        public static ImmutableByteColor Orchid               => new ImmutableByteColor(0xFF, 0xDA, 0x70, 0xD6);
        public static ImmutableByteColor Goldenrod            => new ImmutableByteColor(0xFF, 0xDA, 0xA5, 0x20);
        public static ImmutableByteColor PaleVioletRed        => new ImmutableByteColor(0xFF, 0xDB, 0x70, 0x93);
        public static ImmutableByteColor Crimson              => new ImmutableByteColor(0xFF, 0xDC, 0x14, 0x3C);
        public static ImmutableByteColor Gainsboro            => new ImmutableByteColor(0xFF, 0xDC, 0xDC, 0xDC);
        public static ImmutableByteColor Plum                 => new ImmutableByteColor(0xFF, 0xDD, 0xA0, 0xDD);
        public static ImmutableByteColor BurlyWood            => new ImmutableByteColor(0xFF, 0xDE, 0xB8, 0x87);
        public static ImmutableByteColor LightCyan            => new ImmutableByteColor(0xFF, 0xE0, 0xFF, 0xFF);
        public static ImmutableByteColor Lavender             => new ImmutableByteColor(0xFF, 0xE6, 0xE6, 0xFA);
        public static ImmutableByteColor DarkSalmon           => new ImmutableByteColor(0xFF, 0xE9, 0x96, 0x7A);
        public static ImmutableByteColor Violet               => new ImmutableByteColor(0xFF, 0xEE, 0x82, 0xEE);
        public static ImmutableByteColor PaleGoldenrod        => new ImmutableByteColor(0xFF, 0xEE, 0xE8, 0xAA);
        public static ImmutableByteColor LightCoral           => new ImmutableByteColor(0xFF, 0xF0, 0x80, 0x80);
        public static ImmutableByteColor Khaki                => new ImmutableByteColor(0xFF, 0xF0, 0xE6, 0x8C);
        public static ImmutableByteColor AliceBlue            => new ImmutableByteColor(0xFF, 0xF0, 0xF8, 0xFF);
        public static ImmutableByteColor Honeydew             => new ImmutableByteColor(0xFF, 0xF0, 0xFF, 0xF0);
        public static ImmutableByteColor Azure                => new ImmutableByteColor(0xFF, 0xF0, 0xFF, 0xFF);
        public static ImmutableByteColor SandyBrown           => new ImmutableByteColor(0xFF, 0xF4, 0xA4, 0x60);
        public static ImmutableByteColor Wheat                => new ImmutableByteColor(0xFF, 0xF5, 0xDE, 0xB3);
        public static ImmutableByteColor Beige                => new ImmutableByteColor(0xFF, 0xF5, 0xF5, 0xDC);
        public static ImmutableByteColor WhiteSmoke           => new ImmutableByteColor(0xFF, 0xF5, 0xF5, 0xF5);
        public static ImmutableByteColor MintCream            => new ImmutableByteColor(0xFF, 0xF5, 0xFF, 0xFA);
        public static ImmutableByteColor GhostWhite           => new ImmutableByteColor(0xFF, 0xF8, 0xF8, 0xFF);
        public static ImmutableByteColor Salmon               => new ImmutableByteColor(0xFF, 0xFA, 0x80, 0x72);
        public static ImmutableByteColor AntiqueWhite         => new ImmutableByteColor(0xFF, 0xFA, 0xEB, 0xD7);
        public static ImmutableByteColor Linen                => new ImmutableByteColor(0xFF, 0xFA, 0xF0, 0xE6);
        public static ImmutableByteColor LightGoldenrodYellow => new ImmutableByteColor(0xFF, 0xFA, 0xFA, 0xD2);
        public static ImmutableByteColor OldLace              => new ImmutableByteColor(0xFF, 0xFD, 0xF5, 0xE6);
        public static ImmutableByteColor Red                  => new ImmutableByteColor(0xFF, 0xFF, 0x00, 0x00);
        public static ImmutableByteColor Fuchsia              => new ImmutableByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static ImmutableByteColor Magenta              => new ImmutableByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static ImmutableByteColor DeepPink             => new ImmutableByteColor(0xFF, 0xFF, 0x14, 0x93);
        public static ImmutableByteColor OrangeRed            => new ImmutableByteColor(0xFF, 0xFF, 0x45, 0x00);
        public static ImmutableByteColor Tomato               => new ImmutableByteColor(0xFF, 0xFF, 0x63, 0x47);
        public static ImmutableByteColor HotPink              => new ImmutableByteColor(0xFF, 0xFF, 0x69, 0xB4);
        public static ImmutableByteColor Coral                => new ImmutableByteColor(0xFF, 0xFF, 0x7F, 0x50);
        public static ImmutableByteColor DarkOrange           => new ImmutableByteColor(0xFF, 0xFF, 0x8C, 0x00);
        public static ImmutableByteColor LightSalmon          => new ImmutableByteColor(0xFF, 0xFF, 0xA0, 0x7A);
        public static ImmutableByteColor Orange               => new ImmutableByteColor(0xFF, 0xFF, 0xA5, 0x00);
        public static ImmutableByteColor LightPink            => new ImmutableByteColor(0xFF, 0xFF, 0xB6, 0xC1);
        public static ImmutableByteColor Pink                 => new ImmutableByteColor(0xFF, 0xFF, 0xC0, 0xCB);
        public static ImmutableByteColor Gold                 => new ImmutableByteColor(0xFF, 0xFF, 0xD7, 0x00);
        public static ImmutableByteColor PeachPuff            => new ImmutableByteColor(0xFF, 0xFF, 0xDA, 0xB9);
        public static ImmutableByteColor NavajoWhite          => new ImmutableByteColor(0xFF, 0xFF, 0xDE, 0xAD);
        public static ImmutableByteColor Moccasin             => new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xB5);
        public static ImmutableByteColor Bisque               => new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xC4);
        public static ImmutableByteColor MistyRose            => new ImmutableByteColor(0xFF, 0xFF, 0xE4, 0xE1);
        public static ImmutableByteColor BlanchedAlmond       => new ImmutableByteColor(0xFF, 0xFF, 0xEB, 0xCD);
        public static ImmutableByteColor PapayaWhip           => new ImmutableByteColor(0xFF, 0xFF, 0xEF, 0xD5);
        public static ImmutableByteColor LavenderBlush        => new ImmutableByteColor(0xFF, 0xFF, 0xF0, 0xF5);
        public static ImmutableByteColor SeaShell             => new ImmutableByteColor(0xFF, 0xFF, 0xF5, 0xEE);
        public static ImmutableByteColor Cornsilk             => new ImmutableByteColor(0xFF, 0xFF, 0xF8, 0xDC);
        public static ImmutableByteColor LemonChiffon         => new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xCD);
        public static ImmutableByteColor FloralWhite          => new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xF0);
        public static ImmutableByteColor Snow                 => new ImmutableByteColor(0xFF, 0xFF, 0xFA, 0xFA);
        public static ImmutableByteColor Yellow               => new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0x00);
        public static ImmutableByteColor LightYellow          => new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xE0);
        public static ImmutableByteColor Ivory                => new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xF0);
        public static ImmutableByteColor White                => new ImmutableByteColor(0xFF, 0xFF, 0xFF, 0xFF);
    }

    public static class ColorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableByteColor ToImmutableByteColor(this Color src)
            => new ImmutableByteColor(src.A, src.R, src.G, src.B);
    }
}