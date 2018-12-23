using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface INodeItem : IHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        bool IsRequireVisualTest { get; }

        Size Size { get; set; }

        BiaNodePortLayout Layout { get; }
    }
}