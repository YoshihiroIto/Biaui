using System.ComponentModel;
using System.Windows;

namespace Biaui.NodeEditor
{
    public interface INodeItem : INotifyPropertyChanged
    {
        Point Pos { get; set; }
    }
}