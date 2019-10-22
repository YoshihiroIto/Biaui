using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls;
using Biaui.Controls.NodeEditor;

namespace Biaui.Internals
{
    internal static class Boxes
    {
        internal static readonly object Thickness0 = new Thickness(0);

        internal static readonly object BoolTrue = true;
        internal static readonly object BoolFalse = false;

        internal static readonly object Int0 = 0;
        internal static readonly object Int1 = 1;
        internal static readonly object Int2 = 2;
        internal static readonly object Int3 = 3;
        internal static readonly object Int4 = 4;
        internal static readonly object Int5 = 5;
        internal static readonly object Int6 = 6;
        internal static readonly object Int7 = 7;
        internal static readonly object IntMax = int.MaxValue;

        internal static readonly object Float0 = 0.0f;
        internal static readonly object Float1 = 1.0f;
        internal static readonly object Float2 = 2.0f;
        internal static readonly object Float3 = 3.0f;
        internal static readonly object Float4 = 4.0f;
        internal static readonly object Float5 = 5.0f;
        internal static readonly object Float6 = 6.0f;
        internal static readonly object Float7 = 7.0f;

        internal static readonly object Double0 = 0.0;
        internal static readonly object Double1 = 1.0;
        internal static readonly object Double2 = 2.0;
        internal static readonly object Double3 = 3.0;
        internal static readonly object Double4 = 4.0;
        internal static readonly object Double5 = 5.0;
        internal static readonly object Double6 = 6.0;
        internal static readonly object Double7 = 7.0;
        internal static readonly object Double24 = 24.0;
        internal static readonly object DoubleMin = double.MinValue;
        internal static readonly object DoubleMax = double.MaxValue;

        internal static readonly object ColorRed = Colors.Red;
        internal static readonly object ColorWhite = Colors.White;
        internal static readonly object ColorBlack = Colors.Black;
        internal static readonly object ColorTransparent = Colors.Transparent;

        internal static readonly object Point00 = new Point(0, 0);
        internal static readonly object Size11 = new Size(1, 1);
        internal static readonly object Rect0 = new Rect(0, 0, 0, 0);
        internal static readonly object ImmutableRect0 = new ImmutableRect(0, 0, 0, 0);

        internal static readonly object NumberModeSimple = BiaNumberEditorMode.Simple;
        internal static readonly object NumberModeWideRange = BiaNumberEditorMode.WideRange;

        internal static readonly object NodeEditorNodeLinkStyleAxisAlign = BiaNodeEditorNodeLinkStyle.AxisAlign;
        internal static readonly object NodeEditorNodeLinkStyleBezierCurve = BiaNodeEditorNodeLinkStyle.BezierCurve;

        internal static readonly object ConstantsBasicCornerRadiusPrim = Biaui.Constants.BasicCornerRadiusPrim;

        // ReSharper disable InconsistentNaming
        internal static readonly object WindowCloseButtonBehavior_Normal = WindowCloseButtonBehavior.Normal;

        internal static readonly object WindowAction_None = BiaWindowAction.None;
        internal static readonly object WindowAction_Active = BiaWindowAction.Active;
        internal static readonly object WindowAction_Close = BiaWindowAction.Close;
        internal static readonly object WindowAction_Normalize = BiaWindowAction.Normalize;
        internal static readonly object WindowAction_Maximize = BiaWindowAction.Maximize;
        internal static readonly object WindowAction_Minimize = BiaWindowAction.Minimize;
        // ReSharper restore InconsistentNaming

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Bool(bool i) => i ? BoolTrue : BoolFalse;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Int(int i)
        {
            switch (i)
            {
                case 0: return Int0;
                case 1: return Int1;
                case 2: return Int2;
                case 3: return Int3;
                case 4: return Int4;
                case 5: return Int5;
                case 6: return Int6;
                case 7: return Int7;
                default:
                    //Debug.WriteLine($"Boxes.Int:{i}");
                    return i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Float(float i)
        {
            switch (i)
            {
                case 0.0f: return Float0;
                case 1.0f: return Float1;
                case 2.0f: return Float2;
                case 3.0f: return Float3;
                case 4.0f: return Float4;
                case 5.0f: return Float5;
                case 6.0f: return Float6;
                case 7.0f: return Float7;
                default:
                    //Debug.WriteLine($"Boxes.Float:{i}");
                    return i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Double(double i)
        {
            switch (i)
            {
                case 0.0: return Double0;
                case 1.0: return Double1;
                case 2.0: return Double2;
                case 3.0: return Double3;
                case 4.0: return Double4;
                case 5.0: return Double5;
                case 6.0: return Double6;
                case 7.0: return Double7;
                default:
                    //Debug.WriteLine($"Boxes.Double:{i}");
                    return i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object NumberEditorMode(BiaNumberEditorMode i)
        {
            switch (i)
            {
                case BiaNumberEditorMode.Simple:
                    return NumberModeSimple;
                case BiaNumberEditorMode.WideRange:
                    return NumberModeWideRange;
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object NodeEditorNodeLinkStyle(BiaNodeEditorNodeLinkStyle i)
        {
            switch (i)
            {
                case BiaNodeEditorNodeLinkStyle.AxisAlign:
                    return NodeEditorNodeLinkStyleAxisAlign;
                case BiaNodeEditorNodeLinkStyle.BezierCurve:
                    return NodeEditorNodeLinkStyleBezierCurve;
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object WindowAction(BiaWindowAction a)
        {
            switch (a)
            {
                case BiaWindowAction.None:
                    return WindowAction_None;
                case BiaWindowAction.Active:
                    return WindowAction_Active;
                case BiaWindowAction.Close:
                    return WindowAction_Close;
                case BiaWindowAction.Normalize:
                    return WindowAction_Normalize;
                case BiaWindowAction.Maximize:
                    return WindowAction_Maximize;
                case BiaWindowAction.Minimize:
                    return WindowAction_Minimize;
                default:
                    throw new ArgumentOutOfRangeException(nameof(a), a, null);
            }
        }
    }
}