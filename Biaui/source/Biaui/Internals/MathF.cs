#if !NETCOREAPP
using System;
using System.Runtime.CompilerServices;

namespace Biaui.Internals
{
    internal static class MathF
    {
        // ReSharper disable once InconsistentNaming
        public const float PI = 3.14159265358979f;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float d) => (float) Math.Sqrt(d);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x) => (float) Math.Atan2(y, x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float a) => (float) Math.Sin(a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float a) => (float) Math.Cos(a);
    }
}
#endif
