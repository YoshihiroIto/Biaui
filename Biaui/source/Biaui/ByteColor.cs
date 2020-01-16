using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Biaui
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ByteColor : IEquatable<ByteColor>
    {
        public byte A
        {
            get => _A;
            set => _A = value;
        }

        public byte R
        {
            get => _R;
            set => _R = value;
        }

        public byte G
        {
            get => _G;
            set => _G = value;
        }

        public byte B
        {
            get => _B;
            set => _B = value;
        }

        // for XAML
        public string Argb
        {
            set => _argb = ParseArgb(value);
        }

        [FieldOffset(3)] private byte _A;
        [FieldOffset(2)] private byte _R;
        [FieldOffset(1)] private byte _G;
        [FieldOffset(0)] private byte _B;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [FieldOffset(0)] private int _argb;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteColor(byte a, byte r, byte g, byte b)
        {
            _argb = 0;
            _A = a;
            _R = r;
            _G = g;
            _B = b;
        }

        public long HashCode => _argb;

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _argb;
        }

        public Color ToColor() => Color.FromArgb(_A, _R, _G, _B);
        public Point3D ToPoint3D() => new Point3D(_R * (1.0 / 255.0), _G * (1.0 / 255.0), _B * (1.0 / 255.0));
        public Point4D ToPoint4D() => new Point4D(_R * (1.0 / 255.0), _G * (1.0 / 255.0), _B * (1.0 / 255.0), _A * (1.0 / 255.0));

        public static bool operator ==(ByteColor color1, ByteColor color2) => color1._argb == color2._argb;
        public static bool operator !=(ByteColor color1, ByteColor color2) => color1._argb != color2._argb;

        public bool Equals(ByteColor other) => _argb == other._argb;
        public override bool Equals(object? obj) => obj is ByteColor other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ParseArgb(string argb)
        {
            ReadOnlySpan<byte> table = new byte[]
            {
                // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                // 0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15
                // , 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            
            // 範囲外チェックはReadOnlySpanに任せる

            return (table[argb[0] - '0'] << (7 * 4)) |
                   (table[argb[1] - '0'] << (6 * 4)) |
                   (table[argb[2] - '0'] << (5 * 4)) |
                   (table[argb[3] - '0'] << (4 * 4)) |
                   (table[argb[4] - '0'] << (3 * 4)) |
                   (table[argb[5] - '0'] << (2 * 4)) |
                   (table[argb[6] - '0'] << (1 * 4)) |
                   (table[argb[7] - '0'] << (0 * 4));
        }
     
        //
        public static ByteColor Transparent => new ByteColor(0x00, 0xFF, 0xFF, 0xFF);
        public static ByteColor Black => new ByteColor(0xFF, 0x00, 0x00, 0x00);
        public static ByteColor Navy => new ByteColor(0xFF, 0x00, 0x00, 0x80);
        public static ByteColor DarkBlue => new ByteColor(0xFF, 0x00, 0x00, 0x8B);
        public static ByteColor MediumBlue => new ByteColor(0xFF, 0x00, 0x00, 0xCD);
        public static ByteColor Blue => new ByteColor(0xFF, 0x00, 0x00, 0xFF);
        public static ByteColor DarkGreen => new ByteColor(0xFF, 0x00, 0x64, 0x00);
        public static ByteColor Green => new ByteColor(0xFF, 0x00, 0x80, 0x00);
        public static ByteColor Teal => new ByteColor(0xFF, 0x00, 0x80, 0x80);
        public static ByteColor DarkCyan => new ByteColor(0xFF, 0x00, 0x8B, 0x8B);
        public static ByteColor DeepSkyBlue => new ByteColor(0xFF, 0x00, 0xBF, 0xFF);
        public static ByteColor DarkTurquoise => new ByteColor(0xFF, 0x00, 0xCE, 0xD1);
        public static ByteColor MediumSpringGreen => new ByteColor(0xFF, 0x00, 0xFA, 0x9A);
        public static ByteColor Lime => new ByteColor(0xFF, 0x00, 0xFF, 0x00);
        public static ByteColor SpringGreen => new ByteColor(0xFF, 0x00, 0xFF, 0x7F);
        public static ByteColor Aqua => new ByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static ByteColor Cyan => new ByteColor(0xFF, 0x00, 0xFF, 0xFF);
        public static ByteColor MidnightBlue => new ByteColor(0xFF, 0x19, 0x19, 0x70);
        public static ByteColor DodgerBlue => new ByteColor(0xFF, 0x1E, 0x90, 0xFF);
        public static ByteColor LightSeaGreen => new ByteColor(0xFF, 0x20, 0xB2, 0xAA);
        public static ByteColor ForestGreen => new ByteColor(0xFF, 0x22, 0x8B, 0x22);
        public static ByteColor SeaGreen => new ByteColor(0xFF, 0x2E, 0x8B, 0x57);
        public static ByteColor DarkSlateGray => new ByteColor(0xFF, 0x2F, 0x4F, 0x4F);
        public static ByteColor LimeGreen => new ByteColor(0xFF, 0x32, 0xCD, 0x32);
        public static ByteColor MediumSeaGreen => new ByteColor(0xFF, 0x3C, 0xB3, 0x71);
        public static ByteColor Turquoise => new ByteColor(0xFF, 0x40, 0xE0, 0xD0);
        public static ByteColor RoyalBlue => new ByteColor(0xFF, 0x41, 0x69, 0xE1);
        public static ByteColor SteelBlue => new ByteColor(0xFF, 0x46, 0x82, 0xB4);
        public static ByteColor DarkSlateBlue => new ByteColor(0xFF, 0x48, 0x3D, 0x8B);
        public static ByteColor MediumTurquoise => new ByteColor(0xFF, 0x48, 0xD1, 0xCC);
        public static ByteColor Indigo => new ByteColor(0xFF, 0x4B, 0x00, 0x82);
        public static ByteColor DarkOliveGreen => new ByteColor(0xFF, 0x55, 0x6B, 0x2F);
        public static ByteColor CadetBlue => new ByteColor(0xFF, 0x5F, 0x9E, 0xA0);
        public static ByteColor CornflowerBlue => new ByteColor(0xFF, 0x64, 0x95, 0xED);
        public static ByteColor MediumAquamarine => new ByteColor(0xFF, 0x66, 0xCD, 0xAA);
        public static ByteColor DimGray => new ByteColor(0xFF, 0x69, 0x69, 0x69);
        public static ByteColor SlateBlue => new ByteColor(0xFF, 0x6A, 0x5A, 0xCD);
        public static ByteColor OliveDrab => new ByteColor(0xFF, 0x6B, 0x8E, 0x23);
        public static ByteColor SlateGray => new ByteColor(0xFF, 0x70, 0x80, 0x90);
        public static ByteColor LightSlateGray => new ByteColor(0xFF, 0x77, 0x88, 0x99);
        public static ByteColor MediumSlateBlue => new ByteColor(0xFF, 0x7B, 0x68, 0xEE);
        public static ByteColor LawnGreen => new ByteColor(0xFF, 0x7C, 0xFC, 0x00);
        public static ByteColor Chartreuse => new ByteColor(0xFF, 0x7F, 0xFF, 0x00);
        public static ByteColor Aquamarine => new ByteColor(0xFF, 0x7F, 0xFF, 0xD4);
        public static ByteColor Maroon => new ByteColor(0xFF, 0x80, 0x00, 0x00);
        public static ByteColor Purple => new ByteColor(0xFF, 0x80, 0x00, 0x80);
        public static ByteColor Olive => new ByteColor(0xFF, 0x80, 0x80, 0x00);
        public static ByteColor Gray => new ByteColor(0xFF, 0x80, 0x80, 0x80);
        public static ByteColor SkyBlue => new ByteColor(0xFF, 0x87, 0xCE, 0xEB);
        public static ByteColor LightSkyBlue => new ByteColor(0xFF, 0x87, 0xCE, 0xFA);
        public static ByteColor BlueViolet => new ByteColor(0xFF, 0x8A, 0x2B, 0xE2);
        public static ByteColor DarkRed => new ByteColor(0xFF, 0x8B, 0x00, 0x00);
        public static ByteColor DarkMagenta => new ByteColor(0xFF, 0x8B, 0x00, 0x8B);
        public static ByteColor SaddleBrown => new ByteColor(0xFF, 0x8B, 0x45, 0x13);
        public static ByteColor DarkSeaGreen => new ByteColor(0xFF, 0x8F, 0xBC, 0x8F);
        public static ByteColor LightGreen => new ByteColor(0xFF, 0x90, 0xEE, 0x90);
        public static ByteColor MediumPurple => new ByteColor(0xFF, 0x93, 0x70, 0xDB);
        public static ByteColor DarkViolet => new ByteColor(0xFF, 0x94, 0x00, 0xD3);
        public static ByteColor PaleGreen => new ByteColor(0xFF, 0x98, 0xFB, 0x98);
        public static ByteColor DarkOrchid => new ByteColor(0xFF, 0x99, 0x32, 0xCC);
        public static ByteColor YellowGreen => new ByteColor(0xFF, 0x9A, 0xCD, 0x32);
        public static ByteColor Sienna => new ByteColor(0xFF, 0xA0, 0x52, 0x2D);
        public static ByteColor Brown => new ByteColor(0xFF, 0xA5, 0x2A, 0x2A);
        public static ByteColor DarkGray => new ByteColor(0xFF, 0xA9, 0xA9, 0xA9);
        public static ByteColor LightBlue => new ByteColor(0xFF, 0xAD, 0xD8, 0xE6);
        public static ByteColor GreenYellow => new ByteColor(0xFF, 0xAD, 0xFF, 0x2F);
        public static ByteColor PaleTurquoise => new ByteColor(0xFF, 0xAF, 0xEE, 0xEE);
        public static ByteColor LightSteelBlue => new ByteColor(0xFF, 0xB0, 0xC4, 0xDE);
        public static ByteColor PowderBlue => new ByteColor(0xFF, 0xB0, 0xE0, 0xE6);
        public static ByteColor Firebrick => new ByteColor(0xFF, 0xB2, 0x22, 0x22);
        public static ByteColor DarkGoldenrod => new ByteColor(0xFF, 0xB8, 0x86, 0x0B);
        public static ByteColor MediumOrchid => new ByteColor(0xFF, 0xBA, 0x55, 0xD3);
        public static ByteColor RosyBrown => new ByteColor(0xFF, 0xBC, 0x8F, 0x8F);
        public static ByteColor DarkKhaki => new ByteColor(0xFF, 0xBD, 0xB7, 0x6B);
        public static ByteColor Silver => new ByteColor(0xFF, 0xC0, 0xC0, 0xC0);
        public static ByteColor MediumVioletRed => new ByteColor(0xFF, 0xC7, 0x15, 0x85);
        public static ByteColor IndianRed => new ByteColor(0xFF, 0xCD, 0x5C, 0x5C);
        public static ByteColor Peru => new ByteColor(0xFF, 0xCD, 0x85, 0x3F);
        public static ByteColor Chocolate => new ByteColor(0xFF, 0xD2, 0x69, 0x1E);
        public static ByteColor Tan => new ByteColor(0xFF, 0xD2, 0xB4, 0x8C);
        public static ByteColor LightGray => new ByteColor(0xFF, 0xD3, 0xD3, 0xD3);
        public static ByteColor Thistle => new ByteColor(0xFF, 0xD8, 0xBF, 0xD8);
        public static ByteColor Orchid => new ByteColor(0xFF, 0xDA, 0x70, 0xD6);
        public static ByteColor Goldenrod => new ByteColor(0xFF, 0xDA, 0xA5, 0x20);
        public static ByteColor PaleVioletRed => new ByteColor(0xFF, 0xDB, 0x70, 0x93);
        public static ByteColor Crimson => new ByteColor(0xFF, 0xDC, 0x14, 0x3C);
        public static ByteColor Gainsboro => new ByteColor(0xFF, 0xDC, 0xDC, 0xDC);
        public static ByteColor Plum => new ByteColor(0xFF, 0xDD, 0xA0, 0xDD);
        public static ByteColor BurlyWood => new ByteColor(0xFF, 0xDE, 0xB8, 0x87);
        public static ByteColor LightCyan => new ByteColor(0xFF, 0xE0, 0xFF, 0xFF);
        public static ByteColor Lavender => new ByteColor(0xFF, 0xE6, 0xE6, 0xFA);
        public static ByteColor DarkSalmon => new ByteColor(0xFF, 0xE9, 0x96, 0x7A);
        public static ByteColor Violet => new ByteColor(0xFF, 0xEE, 0x82, 0xEE);
        public static ByteColor PaleGoldenrod => new ByteColor(0xFF, 0xEE, 0xE8, 0xAA);
        public static ByteColor LightCoral => new ByteColor(0xFF, 0xF0, 0x80, 0x80);
        public static ByteColor Khaki => new ByteColor(0xFF, 0xF0, 0xE6, 0x8C);
        public static ByteColor AliceBlue => new ByteColor(0xFF, 0xF0, 0xF8, 0xFF);
        public static ByteColor Honeydew => new ByteColor(0xFF, 0xF0, 0xFF, 0xF0);
        public static ByteColor Azure => new ByteColor(0xFF, 0xF0, 0xFF, 0xFF);
        public static ByteColor SandyBrown => new ByteColor(0xFF, 0xF4, 0xA4, 0x60);
        public static ByteColor Wheat => new ByteColor(0xFF, 0xF5, 0xDE, 0xB3);
        public static ByteColor Beige => new ByteColor(0xFF, 0xF5, 0xF5, 0xDC);
        public static ByteColor WhiteSmoke => new ByteColor(0xFF, 0xF5, 0xF5, 0xF5);
        public static ByteColor MintCream => new ByteColor(0xFF, 0xF5, 0xFF, 0xFA);
        public static ByteColor GhostWhite => new ByteColor(0xFF, 0xF8, 0xF8, 0xFF);
        public static ByteColor Salmon => new ByteColor(0xFF, 0xFA, 0x80, 0x72);
        public static ByteColor AntiqueWhite => new ByteColor(0xFF, 0xFA, 0xEB, 0xD7);
        public static ByteColor Linen => new ByteColor(0xFF, 0xFA, 0xF0, 0xE6);
        public static ByteColor LightGoldenrodYellow => new ByteColor(0xFF, 0xFA, 0xFA, 0xD2);
        public static ByteColor OldLace => new ByteColor(0xFF, 0xFD, 0xF5, 0xE6);
        public static ByteColor Red => new ByteColor(0xFF, 0xFF, 0x00, 0x00);
        public static ByteColor Fuchsia => new ByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static ByteColor Magenta => new ByteColor(0xFF, 0xFF, 0x00, 0xFF);
        public static ByteColor DeepPink => new ByteColor(0xFF, 0xFF, 0x14, 0x93);
        public static ByteColor OrangeRed => new ByteColor(0xFF, 0xFF, 0x45, 0x00);
        public static ByteColor Tomato => new ByteColor(0xFF, 0xFF, 0x63, 0x47);
        public static ByteColor HotPink => new ByteColor(0xFF, 0xFF, 0x69, 0xB4);
        public static ByteColor Coral => new ByteColor(0xFF, 0xFF, 0x7F, 0x50);
        public static ByteColor DarkOrange => new ByteColor(0xFF, 0xFF, 0x8C, 0x00);
        public static ByteColor LightSalmon => new ByteColor(0xFF, 0xFF, 0xA0, 0x7A);
        public static ByteColor Orange => new ByteColor(0xFF, 0xFF, 0xA5, 0x00);
        public static ByteColor LightPink => new ByteColor(0xFF, 0xFF, 0xB6, 0xC1);
        public static ByteColor Pink => new ByteColor(0xFF, 0xFF, 0xC0, 0xCB);
        public static ByteColor Gold => new ByteColor(0xFF, 0xFF, 0xD7, 0x00);
        public static ByteColor PeachPuff => new ByteColor(0xFF, 0xFF, 0xDA, 0xB9);
        public static ByteColor NavajoWhite => new ByteColor(0xFF, 0xFF, 0xDE, 0xAD);
        public static ByteColor Moccasin => new ByteColor(0xFF, 0xFF, 0xE4, 0xB5);
        public static ByteColor Bisque => new ByteColor(0xFF, 0xFF, 0xE4, 0xC4);
        public static ByteColor MistyRose => new ByteColor(0xFF, 0xFF, 0xE4, 0xE1);
        public static ByteColor BlanchedAlmond => new ByteColor(0xFF, 0xFF, 0xEB, 0xCD);
        public static ByteColor PapayaWhip => new ByteColor(0xFF, 0xFF, 0xEF, 0xD5);
        public static ByteColor LavenderBlush => new ByteColor(0xFF, 0xFF, 0xF0, 0xF5);
        public static ByteColor SeaShell => new ByteColor(0xFF, 0xFF, 0xF5, 0xEE);
        public static ByteColor Cornsilk => new ByteColor(0xFF, 0xFF, 0xF8, 0xDC);
        public static ByteColor LemonChiffon => new ByteColor(0xFF, 0xFF, 0xFA, 0xCD);
        public static ByteColor FloralWhite => new ByteColor(0xFF, 0xFF, 0xFA, 0xF0);
        public static ByteColor Snow => new ByteColor(0xFF, 0xFF, 0xFA, 0xFA);
        public static ByteColor Yellow => new ByteColor(0xFF, 0xFF, 0xFF, 0x00);
        public static ByteColor LightYellow => new ByteColor(0xFF, 0xFF, 0xFF, 0xE0);
        public static ByteColor Ivory => new ByteColor(0xFF, 0xFF, 0xFF, 0xF0);
        public static ByteColor White => new ByteColor(0xFF, 0xFF, 0xFF, 0xFF);
    }

    public static class ColorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteColor ToByteColor(this Color src)
            => new ByteColor(src.A, src.R, src.G, src.B);
    }
}