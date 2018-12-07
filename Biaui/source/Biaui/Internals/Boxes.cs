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
        internal static object IntMax = int.MaxValue;

        internal static object Double0 = 0.0;
        internal static object Double1 = 1.0;
        internal static object Double100 = 100.0;

        internal static object ColorRed = Colors.Red;
        internal static object ColorWhite = Colors.White;
        internal static object ColorBlack = Colors.Black;
        internal static object ColorTransparent = Colors.Transparent;

        internal static object Point00 = new Point(0, 0);

        internal static object BiaNumberModeSimple = BiaNumberEditorMode.Simple;

        internal static object ConstantsBasicCornerRadiusPrim = Constants.BasicCornerRadiusPrim;

        internal static object Bool(bool i) => i ? BoolTrue : BoolFalse;

    }
}