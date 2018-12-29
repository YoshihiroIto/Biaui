using System.Windows;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeItemPortPair
    {
        public readonly IBiaNodeItem Item;
        public readonly BiaNodePort Port;

        public BiaNodeItemPortPair(IBiaNodeItem item, BiaNodePort port)
        {
            Item = item;
            Port = port;
        }

        internal Point MakePortPos() => Item.MakePortPos(Port);

        internal BiaNodePort FindPort()
        {
            if (Item.Layout.Ports == null)
                return null;

            if (Item.Layout.Ports.TryGetValue(Port.Id, out var port))
                return port;

            return null;
        }

        internal BiaNodeItemPortIdPair ToItemPortIdPair()
            => new BiaNodeItemPortIdPair(Item, Port.Id);
    }

    public class BiaNodeItemPortIdPair
    {
        public readonly IBiaNodeItem Item;
        public readonly int PortId;

        public BiaNodeItemPortIdPair(IBiaNodeItem item, int portId)
        {
            Item = item;
            PortId = portId;
        }

        internal BiaNodePort FindPort()
        {
            if (Item.Layout.Ports == null)
                return null;

            if (Item.Layout.Ports.TryGetValue(PortId, out var port))
                return port;

            return null;
        }
    }
}