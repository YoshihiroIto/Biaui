namespace Biaui.Internals
{
    internal static class Constants
    {
        internal const double SlotMarkRadius = 10.0;
        internal const double SlotMarkRadiusSq = SlotMarkRadius * SlotMarkRadius;

        internal const double SlotMarkRadius_Highlight = SlotMarkRadius * 1.25;
        internal const double SlotMarkRadius_Highlight2 = SlotMarkRadius_Highlight * 1.1;

        internal const double NodePanelAlignSize = 32.0;

        // ポートマークの半径で一番大きいもの
        internal const double SlotMarkRadius_Max = SlotMarkRadius_Highlight2;

        internal static string TreeViewItemExpanderName = "Expander";
    }
}