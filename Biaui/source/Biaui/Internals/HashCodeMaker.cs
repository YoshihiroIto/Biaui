using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class HashCodeMaker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int To32(long h64)
            => (int) ((h64 & 0xFFFFFFFF) ^ ((h64 >> 32) & 0xFFFFFFFF));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(float x, float y)
        {
            unchecked
            {
                var ix = (long) Unsafe.As<float, int>(ref x);
                var iy = (long) Unsafe.As<float, int>(ref y);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(float x, float y, float z)
        {
            unchecked
            {
                var ix = (long) Unsafe.As<float, int>(ref x);
                var iy = (long) Unsafe.As<float, int>(ref y);
                var iz = (long) Unsafe.As<float, int>(ref z);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(float x, float y, float z, float w)
        {
            unchecked
            {
                var ix = (long) Unsafe.As<float, int>(ref x);
                var iy = (long) Unsafe.As<float, int>(ref y);
                var iz = (long) Unsafe.As<float, int>(ref z);
                var iw = (long) Unsafe.As<float, int>(ref w);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;
                hashCode = (hashCode * 397) ^ iw;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(double x, double y)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(double x, double y, double z)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);
                var iz = Unsafe.As<double, long>(ref z);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(double x, double y, double z, double w)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);
                var iz = Unsafe.As<double, long>(ref z);
                var iw = Unsafe.As<double, long>(ref w);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;
                hashCode = (hashCode * 397) ^ iw;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(double v0, double v1, double v2, double v3, double v4, double v5)
        {
            unchecked
            {
                var iv0 = Unsafe.As<double, long>(ref v0);
                var iv1 = Unsafe.As<double, long>(ref v1);
                var iv2 = Unsafe.As<double, long>(ref v2);
                var iv3 = Unsafe.As<double, long>(ref v3);
                var iv4 = Unsafe.As<double, long>(ref v4);
                var iv5 = Unsafe.As<double, long>(ref v5);

                var hashCode = iv0;

                hashCode = (hashCode * 397) ^ iv1;
                hashCode = (hashCode * 397) ^ iv2;
                hashCode = (hashCode * 397) ^ iv3;
                hashCode = (hashCode * 397) ^ iv4;
                hashCode = (hashCode * 397) ^ iv5;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(double x, double y, double z, bool i)
        {
            Debug.Assert(Unsafe.SizeOf<bool>() == 1);

            var ii = (long) Unsafe.As<bool, byte>(ref i);

            return Make(x, y, z) ^ ii;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(Color color)
        {
            return
                ((long) color.R << 0) |
                ((long) color.G << 8) |
                ((long) color.B << 16) |
                ((long) color.A << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long Make(Color color, double x)
        {
            var ix = Unsafe.As<double, long>(ref x);

            return ix ^ Make(color);
        }
    }
}