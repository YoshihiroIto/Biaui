using System.Windows;
using System.Windows.Media;
using Biaui.Controls.Converters;

namespace Biaui
{
    public static class Constants
    {
        public const double BasicCornerRadiusPrim = 4;
        public static CornerRadius BasicCornerRadius = new CornerRadius(BasicCornerRadiusPrim);

        public const double GroupCornerRadiusPrim = 8;
        public static CornerRadius GroupCornerRadius = new CornerRadius(GroupCornerRadiusPrim);

        public const double BasicOneLineHeight = 24;

        public const double ButtonPaddingX = 10;
        public const double ButtonPaddingY = 3;

        public const double NodeEditor_MinScale = 0.25;
        public const double NodeEditor_MaxScale = 2;

        public static readonly TreeListViewItemMarginConverter TreeListViewItemFirstMarginConverter = new TreeListViewItemMarginConverter
        {
            IsFirstColumn = true,
            Length = 19
        };

        public static readonly TreeListViewItemMarginConverter TreeListViewItemMarginConverter = new TreeListViewItemMarginConverter
        {
            Length = 19
        };

        public static Brush CheckerBrush => _CheckerBrush ??= (Brush) Application.Current.TryFindResource("CheckerBrush");

        private static Brush? _CheckerBrush;
    }

    public enum BiaWindowCloseButtonBehavior
    {
        Normal,
        DoNothing
    }
    
    public enum BiaWindowAction
    {
		None,
		Active,
		Close,
		Normalize,
		Maximize,
		Minimize
    }
    
    public enum TextTrimmingMode
    {
        None,
        Standard,
        Filepath
    }
}