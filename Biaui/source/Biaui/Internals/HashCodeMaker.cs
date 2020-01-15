using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class HashCodeMaker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(float x, float y)
        {
            unchecked
            {
                var ix = Unsafe.As<float, int>(ref x);
                var iy = Unsafe.As<float, int>(ref y);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(float x, float y, float z)
        {
            unchecked
            {
                var ix = Unsafe.As<float, int>(ref x);
                var iy = Unsafe.As<float, int>(ref y);
                var iz = Unsafe.As<float, int>(ref z);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(float x, float y, float z, float w)
        {
            unchecked
            {
                var ix = Unsafe.As<float, int>(ref x);
                var iy = Unsafe.As<float, int>(ref y);
                var iz = Unsafe.As<float, int>(ref z);
                var iw = Unsafe.As<float, int>(ref w);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;
                hashCode = (hashCode * 397) ^ iw;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(double x, double y)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(double x, double y, double z)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);
                var iz = Unsafe.As<double, long>(ref z);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(double x, double y, double z, double w)
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

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(double v0, double v1, double v2, double v3, double v4, double v5, double v6, double v7)
        {
            unchecked
            {
                var iv0 = Unsafe.As<double, long>(ref v0);
                var iv1 = Unsafe.As<double, long>(ref v1);
                var iv2 = Unsafe.As<double, long>(ref v2);
                var iv3 = Unsafe.As<double, long>(ref v3);
                var iv4 = Unsafe.As<double, long>(ref v4);
                var iv5 = Unsafe.As<double, long>(ref v5);
                var iv6 = Unsafe.As<double, long>(ref v6);
                var iv7 = Unsafe.As<double, long>(ref v7);

                var hashCode = iv0;

                hashCode = (hashCode * 397) ^ iv1;
                hashCode = (hashCode * 397) ^ iv2;
                hashCode = (hashCode * 397) ^ iv3;
                hashCode = (hashCode * 397) ^ iv4;
                hashCode = (hashCode * 397) ^ iv5;
                hashCode = (hashCode * 397) ^ iv6;
                hashCode = (hashCode * 397) ^ iv7;

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(double x, double y, double z, bool i)
        {
            Debug.Assert(Unsafe.SizeOf<bool>() == 1);

            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);
                var iy = Unsafe.As<double, long>(ref y);
                var iz = Unsafe.As<double, long>(ref z);
                var ii = (int) Unsafe.As<bool, byte>(ref i);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ iy;
                hashCode = (hashCode * 397) ^ iz;

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32) ^ ii;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Make(in Color color, double x)
        {
            unchecked
            {
                var ix = Unsafe.As<double, long>(ref x);

                var hashCode = ix;

                hashCode = (hashCode * 397) ^ color.R;
                hashCode = (hashCode * 397) ^ color.G;
                hashCode = (hashCode * 397) ^ color.B;
                hashCode = (hashCode * 397) ^ color.A;

                return (int) (hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }
    }
}