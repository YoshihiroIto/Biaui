using System.Windows;
using System.Windows.Media;
using Biaui.Controls.Converters;

namespace Biaui;

public static class Constants
{
    public const double BasicCornerRadiusPrim = 4d;
    public static CornerRadius BasicCornerRadius = new (BasicCornerRadiusPrim);

    public const double GroupCornerRadiusPrim = 8d;
    public static CornerRadius GroupCornerRadius = new (GroupCornerRadiusPrim);

    public const double BasicOneLineHeight = 24d;

    public const double ButtonPaddingX = 10d;
    public const double ButtonPaddingY = 3d;

    public static readonly TreeListViewItemMarginConverter TreeListViewItemFirstMarginConverter = new()
    {
        IsFirstColumn = true,
        Length = 19
    };

    public static readonly TreeListViewItemMarginConverter TreeListViewItemMarginConverter = new()
    {
        Length = 19
    };

    public static readonly IndentToMarginConverter LengthConverter = new ();

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

public enum BiaTextTrimmingMode
{
    None,
    Standard,
    Filepath
}
