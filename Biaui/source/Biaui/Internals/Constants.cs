namespace Biaui.Internals
{
    internal static class Constants
    {
        internal const double PortMarkRadius = 10.0;
        internal const double PortMarkRadiusSq = PortMarkRadius * PortMarkRadius;

        internal const double PortMarkRadius_Highlight = PortMarkRadius * 1.25;
        internal const double PortMarkRadius_Highlight2 = PortMarkRadius_Highlight * 1.1;

        internal const double NodePanelAlignSize = 32.0;

        // ポートマークの半径で一番大きいもの
        internal const double PortMarkRadius_Max = PortMarkRadius_Highlight2;
    }
}