using System;
using System.Diagnostics;
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
        internal static double Clamp01(double value)
        {
            if (value < 0.0)
                return 0.0;

            if (value > 1.0)
                return 1.0;

            return value;
        }

        /// <summary>
        /// value, min, max
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Clamp(in this ValueTuple<double, double, double> value)
        {
            Debug.Assert(value.Item2 <= value.Item3);

            if (value.Item1 < value.Item2)
                return value.Item2;

            if (value.Item1 > value.Item3)
                return value.Item3;

            return value.Item1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (double Min, double Max) MinMax(in this ValueTuple<double, double> value)
            => value.Item1 < value.Item2
                ? (value.Item1, value.Item2)
                : (value.Item2, value.Item1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Max(in this ValueTuple<double, double> value)
            => value.Item1 > value.Item2
                ? value.Item1
                : value.Item2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Min(in this ValueTuple<double, double> value)
            => value.Item1 < value.Item2
                ? value.Item1
                : value.Item2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Max(in this ValueTuple<double, double, double> value)
             => ((value.Item1, value.Item2).Max(), value.Item3).Max();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Min(in this ValueTuple<double, double, double> value)
             => ((value.Item1, value.Item2).Min(), value.Item3).Min();


        /// <summary>
        /// value, min, max
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Clamp(in this ValueTuple<int, int, int> value)
        {
            Debug.Assert(value.Item2 <= value.Item3);

            if (value.Item1 < value.Item2)
                return value.Item2;

            if (value.Item1 > value.Item3)
                return value.Item3;

            return value.Item1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Max(in this ValueTuple<int, int> value)
            => value.Item1 > value.Item2
                ? value.Item1
                : value.Item2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Min(in this ValueTuple<int, int> value)
            => value.Item1 < value.Item2
                ? value.Item1
                : value.Item2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Max(in this ValueTuple<int, int, int> value)
             => ((value.Item1, value.Item2).Max(), value.Item3).Max();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Min(in this ValueTuple<int, int, int> value)
             => ((value.Item1, value.Item2).Min(), value.Item3).Min();
    }
}