using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface IBiaNodeItem : IHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        BiaNodePanelHitType HitType { get; }

        Size Size { get; set; }

        BiaNodePortLayout Layout { get; }

        object InternalData { get; set; }
    }

    public enum BiaNodePanelHitType
    {
        Rectangle,
        Circle,
        Visual
    }
}