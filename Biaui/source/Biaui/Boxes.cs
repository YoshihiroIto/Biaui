using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Biaui.Controls;
using Biaui.Controls.NodeEditor;
using Biaui.Internals;

namespace Biaui
{
    public static class Boxes
    {
        public static readonly object Thickness0 = new Thickness(0);

        public static readonly object BoolTrue = true;
        public static readonly object BoolFalse = false;

        public static readonly object Int0 = 0;
        public static readonly object Int1 = 1;
        public static readonly object Int2 = 2;
        public static readonly object Int3 = 3;
        public static readonly object Int4 = 4;
        public static readonly object Int5 = 5;
        public static readonly object Int6 = 6;
        public static readonly object Int7 = 7;
        public static readonly object IntMax = int.MaxValue;

        public static readonly object Float0 = 0.0f;
        public static readonly object Float1 = 1.0f;
        public static readonly object Float2 = 2.0f;
        public static readonly object Float3 = 3.0f;
        public static readonly object Float4 = 4.0f;
        public static readonly object Float5 = 5.0f;
        public static readonly object Float6 = 6.0f;
        public static readonly object Float7 = 7.0f;

        public static readonly object Double0 = 0.0;
        public static readonly object Double1 = 1.0;
        public static readonly object Double2 = 2.0;
        public static readonly object Double3 = 3.0;
        public static readonly object Double4 = 4.0;
        public static readonly object Double5 = 5.0;
        public static readonly object Double6 = 6.0;
        public static readonly object Double7 = 7.0;
        public static readonly object Double16 = 16.0;
        public static readonly object Double24 = 24.0;
        public static readonly object DoubleMin = double.MinValue;
        public static readonly object DoubleMax = double.MaxValue;

        public static readonly object ColorRed = Colors.Red;
        public static readonly object ColorWhite = Colors.White;
        public static readonly object ColorBlack = Colors.Black;
        public static readonly object ColorTransparent = Colors.Transparent;
        
        public static readonly object ByteColorRed = ByteColor.Red;
        public static readonly object ByteColorWhite = ByteColor.White;
        public static readonly object ByteColorBlack = ByteColor.Black;
        public static readonly object ByteColorTransparent = ByteColor.Transparent;
        
        public static readonly object Point3dRed = new Point3D(1.0, 0.0, 0.0);

        public static readonly object Point00 = new Point(0, 0);
        public static readonly object Size11 = new Size(1, 1);
        public static readonly object Rect0 = new Rect(0, 0, 0, 0);
        public static readonly object ImmutableRect0 = new ImmutableRect_double(0, 0, 0, 0);

        public static readonly object NumberModeSimple = BiaNumberEditorMode.Simple;
        public static readonly object NumberModeWideRange = BiaNumberEditorMode.WideRange;

        public static readonly object NodeEditorNodeLinkStyleAxisAlign = BiaNodeEditorNodeLinkStyle.AxisAlign;
        public static readonly object NodeEditorNodeLinkStyleBezierCurve = BiaNodeEditorNodeLinkStyle.BezierCurve;

        public static readonly object ConstantsBasicCornerRadiusPrim = Constants.BasicCornerRadiusPrim;

        // ReSharper disable InconsistentNaming
        public static readonly object WindowCloseButtonBehavior_Normal = WindowCloseButtonBehavior.Normal;

        public static readonly object WindowAction_None = BiaWindowAction.None;
        public static readonly object WindowAction_Active = BiaWindowAction.Active;
        public static readonly object WindowAction_Close = BiaWindowAction.Close;
        public static readonly object WindowAction_Normalize = BiaWindowAction.Normalize;
        public static readonly object WindowAction_Maximize = BiaWindowAction.Maximize;
        public static readonly object WindowAction_Minimize = BiaWindowAction.Minimize;

        public static readonly object ToggleButtonBehavior_Normal = BiaToggleButtonBehavior.Normal;
        public static readonly object ToggleButtonBehavior_RadioButton = BiaToggleButtonBehavior.RadioButton;
        // ReSharper restore InconsistentNaming

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Bool(bool i) => i ? BoolTrue : BoolFalse;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Int(int i)
        {
            return i switch
            {
                0 => Int0,
                1 => Int1,
                2 => Int2,
                3 => Int3,
                4 => Int4,
                5 => Int5,
                6 => Int6,
                7 => Int7,
                _ => i
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Float(float i)
        {
            return i switch
            {
                0.0f => Float0,
                1.0f => Float1,
                2.0f => Float2,
                3.0f => Float3,
                4.0f => Float4,
                5.0f => Float5,
                6.0f => Float6,
                7.0f => Float7,
                _ => i
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Double(double i)
        {
            return i switch
            {
                0.0 => Double0,
                1.0 => Double1,
                2.0 => Double2,
                3.0 => Double3,
                4.0 => Double4,
                5.0 => Double5,
                6.0 => Double6,
                7.0 => Double7,
                _ => i
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object NumberEditorMode(BiaNumberEditorMode i)
        {
            return i switch
            {
                BiaNumberEditorMode.Simple => NumberModeSimple,
                BiaNumberEditorMode.WideRange => NumberModeWideRange,
                _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object NodeEditorNodeLinkStyle(BiaNodeEditorNodeLinkStyle i)
        {
            return i switch
            {
                BiaNodeEditorNodeLinkStyle.AxisAlign => NodeEditorNodeLinkStyleAxisAlign,
                BiaNodeEditorNodeLinkStyle.BezierCurve => NodeEditorNodeLinkStyleBezierCurve,
                _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object WindowAction(BiaWindowAction a)
        {
            return a switch
            {
                BiaWindowAction.None => WindowAction_None,
                BiaWindowAction.Active => WindowAction_Active,
                BiaWindowAction.Close => WindowAction_Close,
                BiaWindowAction.Normalize => WindowAction_Normalize,
                BiaWindowAction.Maximize => WindowAction_Maximize,
                BiaWindowAction.Minimize => WindowAction_Minimize,
                _ => throw new ArgumentOutOfRangeException(nameof(a), a, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ToggleButtonBehavior(BiaToggleButtonBehavior a)
        {
            return a switch
            {
                BiaToggleButtonBehavior.Normal => ToggleButtonBehavior_Normal,
                BiaToggleButtonBehavior.RadioButton => ToggleButtonBehavior_RadioButton,
                _ => throw new ArgumentOutOfRangeException(nameof(a), a, null)
            };
        }
    }
}