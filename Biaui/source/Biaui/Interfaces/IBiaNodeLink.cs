using System.ComponentModel;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        IBiaNodeItem Item1 { get; }

        string Item1PortId { get; }

        IBiaNodeItem Item2 { get; }

        string Item2PortId { get; }

        object InternalData { get; set; }
    }
}