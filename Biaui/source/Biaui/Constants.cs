using System.Windows;
using Biaui.Controls.Converters;

namespace Biaui
{
    public static class Constants
    {
        public static double BasicCornerRadiusPrim = 4;
        public static CornerRadius BasicCornerRadius = new CornerRadius(BasicCornerRadiusPrim);

        public static double GroupCornerRadiusPrim = 8;
        public static CornerRadius GroupCornerRadius = new CornerRadius(GroupCornerRadiusPrim);

        public static double BasicOneLineHeight = 24;

        public static double ButtonPaddingX = 10;
        public static double ButtonPaddingY = 3;

        public static double NodeEditor_MinScale = 0.05;
        public static double NodeEditor_MaxScale = 2;

        public static TreeListViewItemMarginConverter TreeListViewItemFirstMarginConverter = new TreeListViewItemMarginConverter
        {
            IsFirstColumn = true,
            Length = 19
        };

        public static TreeListViewItemMarginConverter TreeListViewItemMarginConverter = new TreeListViewItemMarginConverter
        {
            Length = 19
        };
    }

    public enum WindowCloseButtonBehavior
    {
        Normal,
        DoNothing
    }
}