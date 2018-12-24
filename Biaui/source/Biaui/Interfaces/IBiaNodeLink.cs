using System.ComponentModel;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        IBiaNodeItem Item1 { get; }

        int Item1PortId { get; }

        IBiaNodeItem Item2 { get; }

        int Item2PortId { get; }

        object InternalData { get; set; }
    }
}