using System;
using System.Runtime.CompilerServices;

namespace Biaui.Internals
{
    public static class NumberHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool AreClose(double value1, double value2)
        {
            const double DBL_EPSILON = 2.2204460492503131e-016;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value1 == value2)
                return true;

            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            var delta = value1 - value2;

            return (-eps < delta) && (eps > delta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool AreCloseZero(double value1)
        {
            const double DBL_EPSILON = 2.2204460492503131e-016;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value1 == 0.0)
                return true;

            var eps = (Math.Abs(value1) + 10.0) * DBL_EPSILON;
            var delta = value1;

            return (-eps < delta) && (eps > delta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (double Min, double Max) MinMax(double v0, double v1)
            => v0 < v1
                ? (v0, v1)
                : (v1, v0);
    }
}