using System.Windows;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public readonly struct BiaNodeItemPortPair
    {
        public readonly IBiaNodeItem Item;
        public readonly BiaNodePort Port;

        public BiaNodeItemPortPair(IBiaNodeItem item, BiaNodePort port)
        {
            Item = item;
            Port = port;
        }

        public bool IsNull => Item == null;
        public bool IsNotNull => Item != null;

        internal Point MakePortPos() => Item.MakePortPos(Port);

        internal BiaNodeItemPortIdPair ToItemPortIdPair()
            => new BiaNodeItemPortIdPair(Item, Port.Id);

        // ReSharper disable PossibleNullReferenceException
        public static bool operator ==(in BiaNodeItemPortPair source1, in BiaNodeItemPortPair source2)
            => source1.Item == source2.Item &&
               source1.Port == source2.Port;
        // ReSharper restore PossibleNullReferenceException

        public static bool operator !=(in BiaNodeItemPortPair source1, in BiaNodeItemPortPair source2)
            => !(source1 == source2);

        public bool Equals(BiaNodeItemPortPair other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is BiaNodeItemPortPair other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => Item.GetHashCode() ^
               Port.GetHashCode();
    }

    public readonly struct BiaNodeItemPortIdPair
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

        // ReSharper disable PossibleNullReferenceException
        public static bool operator ==(in BiaNodeItemPortIdPair source1, in BiaNodeItemPortIdPair source2)
            => source1.Item == source2.Item &&
               source1.PortId == source2.PortId;
        // ReSharper restore PossibleNullReferenceException

        public static bool operator !=(in BiaNodeItemPortIdPair source1, in BiaNodeItemPortIdPair source2)
            => !(source1 == source2);

        public bool Equals(BiaNodeItemPortIdPair other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is BiaNodeItemPortIdPair other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => Item.GetHashCode() ^
               PortId.GetHashCode();
    }
}