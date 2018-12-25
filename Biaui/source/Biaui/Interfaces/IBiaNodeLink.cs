using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        IBiaNodeItem Item1 { get; }

        int Item1PortId { get; }

        IBiaNodeItem Item2 { get; }

        int Item2PortId { get; }

        Color Color { get; set; }

        BiaNodeLinkStyle Style { get; set; }

        object InternalData { get; set; }
    }

    [Flags]
    public enum BiaNodeLinkStyle
    {
        None,
        //
        DashedLine
    }
}