using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls;
using Biaui.Controls.NodeEditor;

namespace Biaui.Internals
{
    internal static class Boxes
    {
        internal static object Thickness0 = new Thickness(0);

        internal static object BoolTrue = true;
        internal static object BoolFalse = false;

        internal static object Int0 = 0;
        internal static object Int1 = 1;
        internal static object Int2 = 2;
        internal static object Int3 = 3;
        internal static object Int4 = 4;
        internal static object Int5 = 5;
        internal static object Int6 = 6;
        internal static object Int7 = 7;
        internal static object IntMax = int.MaxValue;

        internal static object Float0 = 0.0f;
        internal static object Float1 = 1.0f;
        internal static object Float2 = 2.0f;
        internal static object Float3 = 3.0f;
        internal static object Float4 = 4.0f;
        internal static object Float5 = 5.0f;
        internal static object Float6 = 6.0f;
        internal static object Float7 = 7.0f;

        internal static object Double0 = 0.0;
        internal static object Double1 = 1.0;
        internal static object Double2 = 2.0;
        internal static object Double3 = 3.0;
        internal static object Double4 = 4.0;
        internal static object Double5 = 5.0;
        internal static object Double6 = 6.0;
        internal static object Double7 = 7.0;
        internal static object Double24 = 24.0;
        internal static object DoubleMin = double.MinValue;
        internal static object DoubleMax = double.MaxValue;

        internal static object ColorRed = Colors.Red;
        internal static object ColorWhite = Colors.White;
        internal static object ColorBlack = Colors.Black;
        internal static object ColorTransparent = Colors.Transparent;

        internal static object Point00 = new Point(0, 0);
        internal static object Size11 = new Size(1, 1);
        internal static object Rect0 = new Rect(0, 0, 0, 0);
        internal static object ImmutableRect0 = new ImmutableRect(0, 0, 0, 0);

        internal static object NumberModeSimple = BiaNumberEditorMode.Simple;
        internal static object NumberModeWideRange = BiaNumberEditorMode.WideRange;

        internal static object NodeEditorNodeLinkStyleAxisAlign = BiaNodeEditorNodeLinkStyle.AxisAlign;
        internal static object NodeEditorNodeLinkStyleBezierCurve = BiaNodeEditorNodeLinkStyle.BezierCurve;

        internal static object ConstantsBasicCornerRadiusPrim = Biaui.Constants.BasicCornerRadiusPrim;

        internal static object WindowCloseButtonBehavior_Normal = WindowCloseButtonBehavior.Normal;

        internal static object WindowAction_None = BiaWindowAction.None;
        internal static object WindowAction_Active = BiaWindowAction.Active;
        internal static object WindowAction_Close = BiaWindowAction.Close;
        internal static object WindowAction_Normalize = BiaWindowAction.Normalize;
        internal static object WindowAction_Maximize = BiaWindowAction.Maximize;
        internal static object WindowAction_Minimize = BiaWindowAction.Minimize;

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
                    Debug.WriteLine($"Boxes.Int:{i}");
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
                    Debug.WriteLine($"Boxes.Float:{i}");
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
                    Debug.WriteLine($"Boxes.Double:{i}");
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