using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class ColorHelper
    {
        internal static Color Lerp(double ratio, Color c1, Color c2)
            => Color.FromArgb(
                (byte)((c2.A - c1.A) * ratio + c1.A),
                (byte)((c2.R - c1.R) * ratio + c1.R),
                (byte)((c2.G - c1.G) * ratio + c1.G),
                (byte)((c2.B - c1.B) * ratio + c1.B));
        
        
        internal static ByteColor Lerp(double ratio, ByteColor c1, ByteColor c2)
            => new ByteColor(
                (byte)((c2.A - c1.A) * ratio + c1.A),
                (byte)((c2.R - c1.R) * ratio + c1.R),
                (byte)((c2.G - c1.G) * ratio + c1.G),
                (byte)((c2.B - c1.B) * ratio + c1.B));
        
    }
}