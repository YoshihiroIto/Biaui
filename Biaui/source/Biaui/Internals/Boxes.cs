using System.Windows;
using System.Windows.Media;
using Biaui.Controls;

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

        internal static object Double0 = 0.0;
        internal static object Double1 = 1.0;
        internal static object Double100 = 100.0;
        internal static object DoubleMin = double.MinValue;
        internal static object DoubleMax = double.MaxValue;

        internal static object ColorRed = Colors.Red;
        internal static object ColorWhite = Colors.White;
        internal static object ColorBlack = Colors.Black;
        internal static object ColorTransparent = Colors.Transparent;

        internal static object Point00 = new Point(0, 0);
        internal static object Size11 = new Size(1, 1);
        internal static object Rect0 = new Rect(0,0,0,0);
        internal static object ImmutableRect0 = new ImmutableRect(0,0,0,0);

        internal static object BiaNumberModeSimple = BiaNumberEditorMode.Simple;

        internal static object ConstantsBasicCornerRadiusPrim = Biaui.Constants.BasicCornerRadiusPrim;

        internal static object WindowCloseButtonBehavior_Normal = WindowCloseButtonBehavior.Normal;

        internal static object Bool(bool i) => i ? BoolTrue : BoolFalse;

    }
}