using System.Runtime.CompilerServices;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui
{
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

        public static readonly DoubleColor Zero = new DoubleColor
        {
            R = 0.0,
            G = 0.0,
            B = 0.0,
            A = 0.0
        };

        public static readonly DoubleColor White = new DoubleColor
        {
            R = 1.0,
            G = 1.0,
            B = 1.0,
            A = 1.0
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
    }
}