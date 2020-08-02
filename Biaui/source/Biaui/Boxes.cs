using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Biaui.Controls;

namespace Biaui
{
    public static class Boxes
    {
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
        public static readonly object Double8 = 8.0;
        public static readonly object Double10 = 10.0;
        public static readonly object Double12 = 12.0;
        public static readonly object Double13 = 13.0;
        public static readonly object Double14 = 14.0;
        public static readonly object Double15 = 15.0;
        public static readonly object Double16 = 16.0;
        public static readonly object Double20 = 20.0;
        public static readonly object Double24 = 24.0;
        public static readonly object Double32 = 32.0;
        public static readonly object Double36 = 36.0;
        public static readonly object Double128 = 128.0;
        public static readonly object Double256 = 256.0;
        public static readonly object Double65536 = 65536.0;
        public static readonly object DoubleMin = double.MinValue;
        public static readonly object DoubleMax = double.MaxValue;

        public static readonly object ByteColorRed = ByteColor.Red;
        public static readonly object ByteColorWhite = ByteColor.White;
        public static readonly object ByteColorBlack = ByteColor.Black;
        public static readonly object ByteColorTransparent = ByteColor.Transparent;

        public static readonly object Point3dRed = new Point3D(1.0, 0.0, 0.0);

        public static readonly object NumberModeSimple = BiaNumberEditorMode.Simple;
        public static readonly object NumberModeWideRange = BiaNumberEditorMode.WideRange;

        public static readonly object BasicOneLineHeight = Constants.BasicOneLineHeight;
        public static readonly object BasicCornerRadiusPrim = Constants.BasicCornerRadiusPrim;
        public static readonly object BasicCornerRadius = Constants.BasicCornerRadius;
        public static readonly object GroupCornerRadius = Constants.GroupCornerRadius;
        public static readonly object GroupCornerRadiusPrim = Constants.GroupCornerRadiusPrim;
        public static readonly object ButtonPaddingX = Constants.ButtonPaddingX;
        public static readonly object ButtonPaddingY = Constants.ButtonPaddingY;

        public static readonly object Thickness0 = new Thickness(0);
        public static readonly object Thickness1 = new Thickness(1);
        public static readonly object Thickness2 = new Thickness(2);
        public static readonly object Thickness3 = new Thickness(3);
        public static readonly object Thickness4 = new Thickness(4);
        public static readonly object Thickness8 = new Thickness(8);
        public static readonly object Thickness10 = new Thickness(10);
        public static readonly object Thickness12 = new Thickness(12);
        public static readonly object Thickness16 = new Thickness(16);
        public static readonly object Thickness24 = new Thickness(24);

        public static readonly object CornerRadius0 = new CornerRadius(0);

        public static readonly object GridLengthStar = new GridLength(1, GridUnitType.Star);
        public static readonly object GridLengthAuto = new GridLength(1, GridUnitType.Auto);

        public static readonly object VisibilityVisible = System.Windows.Visibility.Visible;
        public static readonly object VisibilityCollapsed = System.Windows.Visibility.Collapsed;
        public static readonly object VisibilityHidden = System.Windows.Visibility.Hidden;

        public static readonly object HorizontalAlignmentLeft = HorizontalAlignment.Left;
        public static readonly object HorizontalAlignmentCenter = HorizontalAlignment.Center;
        public static readonly object HorizontalAlignmentRight = HorizontalAlignment.Right;
        public static readonly object HorizontalAlignmentStretch = HorizontalAlignment.Stretch;

        public static readonly object VerticalAlignmentCenter = VerticalAlignment.Center;
        public static readonly object VerticalAlignmentTop = VerticalAlignment.Top;
        public static readonly object VerticalAlignmentBottom = VerticalAlignment.Bottom;
        public static readonly object VerticalAlignmentStretch = VerticalAlignment.Stretch;

        public static readonly object OrientationHorizontal = Orientation.Horizontal;
        public static readonly object OrientationVertical = Orientation.Vertical;

        public static readonly object TextAlignmentLeft    = System.Windows.TextAlignment.Left;
        public static readonly object TextAlignmentRight   = System.Windows.TextAlignment.Right;
        public static readonly object TextAlignmentCenter  = System.Windows.TextAlignment.Center;
        public static readonly object TextAlignmentJustify = System.Windows.TextAlignment.Justify;

        public static readonly object DockLeft = Dock.Left;
        public static readonly object DockTop = Dock.Top;
        public static readonly object DockRight = Dock.Right;
        public static readonly object DockBottom = Dock.Bottom;

        // ReSharper disable InconsistentNaming
        public static readonly object WindowActionNone = BiaWindowAction.None;
        public static readonly object WindowActionActive = BiaWindowAction.Active;
        public static readonly object WindowActionClose = BiaWindowAction.Close;
        public static readonly object WindowActionNormalize = BiaWindowAction.Normalize;
        public static readonly object WindowActionMaximize = BiaWindowAction.Maximize;
        public static readonly object WindowActionMinimize = BiaWindowAction.Minimize;

        public static readonly object ToggleButtonBehaviorNormal = BiaToggleButtonBehavior.Normal;
        public static readonly object ToggleButtonBehaviorRadioButton = BiaToggleButtonBehavior.RadioButton;

        public static readonly object TextTrimmingModeNone = BiaTextTrimmingMode.None;
        public static readonly object TextTrimmingModeStandard = BiaTextTrimmingMode.Standard;
        public static readonly object TextTrimmingModeFilepath = BiaTextTrimmingMode.Filepath;

        public static readonly object WindowCloseButtonBehaviorNormal = BiaWindowCloseButtonBehavior.Normal;
        public static readonly object WindowCloseButtonBehaviorDoNothing = BiaWindowCloseButtonBehavior.DoNothing;
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
                8.0 => Double8,
                10.0 => Double10,
                12.0 => Double12,
                13.0 => Double13,
                14.0 => Double14,
                15.0 => Double15,
                16.0 => Double16,
                20.0 => Double20,
                24.0 => Double24,
                32.0 => Double32,
                36.0 => Double36,
                256.0 => Double256,
                65536.0 => Double65536,
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
        public static object WindowAction(BiaWindowAction a)
        {
            return a switch
            {
                BiaWindowAction.None => WindowActionNone,
                BiaWindowAction.Active => WindowActionActive,
                BiaWindowAction.Close => WindowActionClose,
                BiaWindowAction.Normalize => WindowActionNormalize,
                BiaWindowAction.Maximize => WindowActionMaximize,
                BiaWindowAction.Minimize => WindowActionMinimize,
                _ => throw new ArgumentOutOfRangeException(nameof(a), a, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ToggleButtonBehavior(BiaToggleButtonBehavior a)
        {
            return a switch
            {
                BiaToggleButtonBehavior.Normal => ToggleButtonBehaviorNormal,
                BiaToggleButtonBehavior.RadioButton => ToggleButtonBehaviorRadioButton,
                _ => throw new ArgumentOutOfRangeException(nameof(a), a, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Visibility(Visibility v)
        {
            return v switch
            {
                System.Windows.Visibility.Visible => VisibilityVisible,
                System.Windows.Visibility.Hidden => VisibilityHidden,
                System.Windows.Visibility.Collapsed => VisibilityCollapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object TextAlignment(TextAlignment v)
        {
            return v switch
            {
                System.Windows.TextAlignment.Left => TextAlignmentLeft,
                System.Windows.TextAlignment.Right => TextAlignmentRight,
                System.Windows.TextAlignment.Center => TextAlignmentCenter,
                System.Windows.TextAlignment.Justify => TextAlignmentJustify,
                _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object TextTrimming(BiaTextTrimmingMode v)
        {
            return v switch
            {
                BiaTextTrimmingMode.None => TextTrimmingModeNone,
                BiaTextTrimmingMode.Standard => TextTrimmingModeStandard,
                BiaTextTrimmingMode.Filepath => TextTrimmingModeFilepath,
                _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object WindowCloseButton(BiaWindowCloseButtonBehavior v)
        {
            return v switch
            {
                BiaWindowCloseButtonBehavior.Normal => WindowCloseButtonBehaviorNormal,
                BiaWindowCloseButtonBehavior.DoNothing => WindowCloseButtonBehaviorDoNothing,
                _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
            };
        }
    }
}