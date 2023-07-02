using System.Runtime.CompilerServices;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui;

public struct DoubleColor
{
    public bool Equals(DoubleColor other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is DoubleColor other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        // ReSharper disable NonReadonlyMemberInGetHashCode
        => HashCodeMaker.To32(HashCodeMaker.Make(R, G, B, A));
    // ReSharper restore NonReadonlyMemberInGetHashCode

    public static readonly DoubleColor Zero = new ()
    {
        R = 0d,
        G = 0d,
        B = 0d,
        A = 0d
    };

    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }
    public double A { get; set; }

    public Color Color =>
        Color.FromArgb(
            (byte) (NumberHelper.Clamp01(A) * 0xFF),
            (byte) (NumberHelper.Clamp01(R) * 0xFF),
            (byte) (NumberHelper.Clamp01(G) * 0xFF),
            (byte) (NumberHelper.Clamp01(B) * 0xFF)
        );
    
    public ByteColor ByteColor =>
        new (
            (byte) (NumberHelper.Clamp01(A) * 0xFF),
            (byte) (NumberHelper.Clamp01(R) * 0xFF),
            (byte) (NumberHelper.Clamp01(G) * 0xFF),
            (byte) (NumberHelper.Clamp01(B) * 0xFF)
        );
}
