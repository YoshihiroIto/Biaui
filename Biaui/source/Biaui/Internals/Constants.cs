using System.Runtime.CompilerServices;

namespace Biaui.Internals;

internal static class Constants
{
    internal const double NodePanelAlignSize = 32d;
    internal const string TreeViewItemExpanderName = "Expander";
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double SlotMarkRadius(bool isDesktopSpace) =>
        isDesktopSpace ? 8d : 10d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double SlotMarkRadiusSq(bool isDesktopSpace) =>
        SlotMarkRadius(isDesktopSpace) * SlotMarkRadius(isDesktopSpace);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double SlotMarkRadius_Highlight(bool isDesktopSpace) =>
        SlotMarkRadius(isDesktopSpace) * 1.25d;
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double SlotMarkRadius_Highlight2(bool isDesktopSpace) =>
        SlotMarkRadius_Highlight(isDesktopSpace) * 1.1d;
    
    // ポートマークの半径で一番大きいもの
    internal static readonly double SlotMarkRadiusMax =
        SlotMarkRadius_Highlight2(true);
    
    
    // private const double BaseSlotMarkRadius = 10d;
    //private const double BaseSlotMarkRadiusSq = BaseSlotMarkRadius * BaseSlotMarkRadius;

    // private const double BaseSlotMarkRadius_Highlight = BaseSlotMarkRadius * 1.25d;
    // private const double BaseSlotMarkRadius_Highlight2 = BaseSlotMarkRadius_Highlight * 1.1d;
    
    // ポートマークの半径で一番大きいもの
    //private const double BaseSlotMarkRadius_Max = BaseSlotMarkRadius_Highlight2;
}
