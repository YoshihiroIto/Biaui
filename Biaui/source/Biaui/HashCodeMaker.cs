using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Biaui
{
    public static class HashCodeMaker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int To32(long h64)
            => (int) ((h64 & 0xFFFFFFFF) ^ ((h64 >> 32) & 0xFFFFFFFF));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Make(float x, float y)
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
        public static long Make(float x, float y, float z)
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
        public static long Make(float x, float y, float z, float w)
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
        public static long Make(double x, double y)
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
        public static long Make(double x, double y, double z)
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
        public static long Make(double x, double y, double z, double w)
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
        public static long Make(double v0, double v1, double v2, double v3, double v4, double v5)
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
        public static long Make(double x, double y, double z, bool i)
        {
            Debug.Assert(Unsafe.SizeOf<bool>() == 1);

            var ii = (long) Unsafe.As<bool, byte>(ref i);

            return Make(x, y, z) ^ ii;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Make(ByteColor color)
        {
            return color.HashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Make(ByteColor color, double x)
        {
            var ix = Unsafe.As<double, long>(ref x);

            return ix ^ color.HashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Make(ByteColor color, bool i)
        {
            Debug.Assert(Unsafe.SizeOf<bool>() == 1);

            var ii = (long) Unsafe.As<bool, byte>(ref i);

            return ii ^ color.HashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Make(ReadOnlySpan<char> x)
            => Make(MemoryMarshal.Cast<char, byte>(x));

        public static long Make(ReadOnlySpan<byte> x)
        {
            unchecked
            {
                long hashCode = 0;

                var i8Span = MemoryMarshal.Cast<byte, long>(x);

                foreach (var v in i8Span)
                    hashCode = (hashCode * 397) ^ v;
                
                var remainingLength = x.Length & 7;

                var i1Span = x.Slice(x.Length - remainingLength, remainingLength);
                var i2Span = MemoryMarshal.Cast<byte, ushort>(i1Span);
                var i4Span = MemoryMarshal.Cast<byte, uint>(i1Span);

                switch (remainingLength)
                {
                    case 0:
                        break;
                    
                    case 1:
                        hashCode = (hashCode * 397) ^ i1Span[0];
                        break;
                    
                    case 2:
                        hashCode = (hashCode * 397) ^ i2Span[0];
                        break;
                    
                    case 3:
                        hashCode = (hashCode * 397) ^ i2Span[0];
                        hashCode = (hashCode * 397) ^ i1Span[2];
                        break;
                    
                    case 4:
                        hashCode = (hashCode * 397) ^ i4Span[0];
                        break;
                    
                    case 5:
                        hashCode = (hashCode * 397) ^ i4Span[0];
                        hashCode = (hashCode * 397) ^ i1Span[4];
                        break;
                    
                    case 6:
                        hashCode = (hashCode * 397) ^ i4Span[0];
                        hashCode = (hashCode * 397) ^ i2Span[2];
                        break;
                    
                    case 7:
                        hashCode = (hashCode * 397) ^ i4Span[0];
                        hashCode = (hashCode * 397) ^ i2Span[2];
                        hashCode = (hashCode * 397) ^ i1Span[6];
                        break;
                }

                return hashCode;
            }
        }
    }
}