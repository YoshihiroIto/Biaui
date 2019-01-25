using System;
using System.ComponentModel;
using System.Windows.Media;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        BiaNodeItemPortIdPair ItemPort1 { get; }

        BiaNodeItemPortIdPair ItemPort2 { get; }

        Color Color { get; set; }

        BiaNodeLinkStyle Style { get; set; }

        object InternalData { get; set; }
    }

    [Flags]
    public enum BiaNodeLinkStyle
    {
        None,

        //
        DashedLine = 1 << 0,
        Arrow = 1 << 1
    }
}