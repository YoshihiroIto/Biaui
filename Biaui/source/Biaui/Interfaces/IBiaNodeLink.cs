using System;
using System.ComponentModel;
using System.Windows.Media;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        BiaNodeItemSlotIdPair ItemSlot1 { get; }

        BiaNodeItemSlotIdPair ItemSlot2 { get; }

        Color Color { get; set; }

        BiaNodeLinkStyle Style { get; set; }

        object InternalData { get; set; }

        bool IsVisible { get; }
    }

    public static class BiaNodeLinkExtensions
    {
        public static void Reset(this IBiaNodeLink self)
        {
            self.InternalData = null;
        }
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