using System;
using System.Runtime.CompilerServices;

namespace Biaui.Internals
{
#if !NETCOREAPP3_1
    internal static class MathF
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float v) => (float) Math.Sqrt(v);
    }
#endif
}