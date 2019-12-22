using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class HashCodeMaker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(float x, float y)
        {
            unchecked
            {
                var xp = *(int*) &x;
                var yp = *(int*) &y;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(float x, float y, float z)
        {
            unchecked
            {
                var xp = *(int*) &x;
                var yp = *(int*) &y;
                var zp = *(int*) &z;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;
                hashCode = (hashCode * 397) ^ zp;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(float x, float y, float z, float w)
        {
            unchecked
            {
                var xp = *(int*) &x;
                var yp = *(int*) &y;
                var zp = *(int*) &z;
                var wp = *(int*) &w;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;
                hashCode = (hashCode * 397) ^ zp;
                hashCode = (hashCode * 397) ^ wp;

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(double x, double y)
        {
            unchecked
            {
                var xp = *(long*) &x;
                var yp = *(long*) &y;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(double x, double y, double z)
        {
            unchecked
            {
                var xp = *(long*) &x;
                var yp = *(long*) &y;
                var zp = *(long*) &z;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;
                hashCode = (hashCode * 397) ^ zp;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(double x, double y, double z, double w)
        {
            unchecked
            {
                var xp = *(long*) &x;
                var yp = *(long*) &y;
                var zp = *(long*) &z;
                var wp = *(long*) &w;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;
                hashCode = (hashCode * 397) ^ zp;
                hashCode = (hashCode * 397) ^ wp;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(double v0, double v1, double v2, double v3, double v4, double v5, double v6, double v7)
        {
            unchecked
            {
                // ReSharper disable InconsistentNaming
                var v0p = *(long*) &v0;
                var v1p = *(long*) &v1;
                var v2p = *(long*) &v2;
                var v3p = *(long*) &v3;
                var v4p = *(long*) &v4;
                var v5p = *(long*) &v5;
                var v6p = *(long*) &v6;
                var v7p = *(long*) &v7;
                // ReSharper restore InconsistentNaming

                var hashCode = v0p;

                hashCode = (hashCode * 397) ^ v0p;
                hashCode = (hashCode * 397) ^ v1p;
                hashCode = (hashCode * 397) ^ v2p;
                hashCode = (hashCode * 397) ^ v3p;
                hashCode = (hashCode * 397) ^ v4p;
                hashCode = (hashCode * 397) ^ v5p;
                hashCode = (hashCode * 397) ^ v6p;
                hashCode = (hashCode * 397) ^ v7p;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(double x, double y, double z, bool i)
        {
            unchecked
            {
                var xp = *(long*) &x;
                var yp = *(long*) &y;
                var zp = *(long*) &z;

                var hashCode = xp;

                hashCode = (hashCode * 397) ^ yp;
                hashCode = (hashCode * 397) ^ zp;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32) ^ (i ? 1 : 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int Make(in Color color, double x)
        {
            unchecked
            {
                var xp = *(long*) &x;

                var hashCode = (long) color.R;

                hashCode = (hashCode * 397) ^ color.G;
                hashCode = (hashCode * 397) ^ color.B;
                hashCode = (hashCode * 397) ^ color.A;
                hashCode = (hashCode * 397) ^ xp;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }
    }
}