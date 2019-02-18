using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface IBiaNodeItem : IBiaHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        BiaNodePanelHitType HitType { get; }

        Size Size { get; set; }

        BiaNodeSlotLayout SlotLayout { get; }

        object InternalData { get; set; }
    }

    public enum BiaNodePanelHitType
    {
        Rectangle,
        Circle,
        Visual
    }
}