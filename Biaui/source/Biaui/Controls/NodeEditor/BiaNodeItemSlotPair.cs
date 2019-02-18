using System.Windows;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public readonly struct BiaNodeItemSlotPair
    {
        public readonly IBiaNodeItem Item;
        public readonly BiaNodeSlot Slot;

        public BiaNodeItemSlotPair(IBiaNodeItem item, BiaNodeSlot slot)
        {
            Item = item;
            Slot = slot;
        }

        public bool IsNull => Item == null;
        public bool IsNotNull => Item != null;

        internal Point MakeSlotPos() => Item.MakeSlotPos(Slot);

        internal BiaNodeItemSlotIdPair ToItemSlotIdPair()
            => new BiaNodeItemSlotIdPair(Item, Slot.Id);

        // ReSharper disable PossibleNullReferenceException
        public static bool operator ==(in BiaNodeItemSlotPair source1, in BiaNodeItemSlotPair source2)
            => source1.Item == source2.Item &&
               source1.Slot == source2.Slot;
        // ReSharper restore PossibleNullReferenceException

        public static bool operator !=(in BiaNodeItemSlotPair source1, in BiaNodeItemSlotPair source2)
            => !(source1 == source2);

        public bool Equals(BiaNodeItemSlotPair other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is BiaNodeItemSlotPair other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => Item.GetHashCode() ^
               Slot.GetHashCode();
    }

    public readonly struct BiaNodeItemSlotIdPair
    {
        public readonly IBiaNodeItem Item;
        public readonly int SlotId;

        public BiaNodeItemSlotIdPair(IBiaNodeItem item, int slotId)
        {
            Item = item;
            SlotId = slotId;
        }

        internal BiaNodeSlot FindSlot()
        {
            if (Item.SlotLayout.Slots == null)
                return null;

            if (Item.SlotLayout.Slots.TryGetValue(SlotId, out var slot))
                return slot;

            return null;
        }

        // ReSharper disable PossibleNullReferenceException
        public static bool operator ==(in BiaNodeItemSlotIdPair source1, in BiaNodeItemSlotIdPair source2)
            => source1.Item == source2.Item &&
               source1.SlotId == source2.SlotId;
        // ReSharper restore PossibleNullReferenceException

        public static bool operator !=(in BiaNodeItemSlotIdPair source1, in BiaNodeItemSlotIdPair source2)
            => !(source1 == source2);

        public bool Equals(BiaNodeItemSlotIdPair other)
            => this == other;

        public override bool Equals(object obj)
        {
            if (obj is BiaNodeItemSlotIdPair other)
                return this == other;

            return false;
        }

        public override int GetHashCode()
            => Item.GetHashCode() ^
               SlotId.GetHashCode();
    }
}