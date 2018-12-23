using System.ComponentModel;

namespace Biaui.Interfaces
{
    public interface INodeLink : INotifyPropertyChanged
    {
        INodeItem Item1 { get; }

        string Item1PortId { get; }

        INodeItem Item2 { get; }

        string Item2PortId { get; }

        object InternalData { get; set; }
    }
}