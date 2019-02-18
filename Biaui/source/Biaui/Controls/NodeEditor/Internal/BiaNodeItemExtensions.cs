using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeItemExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static InternalBiaNodeItemData InternalData(this IBiaNodeItem self)
        {
            InternalBiaNodeItemData internalData;

            if (self.InternalData == null)
            {
                internalData = new InternalBiaNodeItemData();
                self.InternalData = internalData;
            }
            else
            {
                internalData = (InternalBiaNodeItemData) self.InternalData;
            }

            return internalData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableRect MakeRect(this IBiaNodeItem self)
            => new ImmutableRect(self.Pos, self.Size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableCircle MakeCircle(this IBiaNodeItem self)
        {
            var size = self.Size;
            return new ImmutableCircle(
                self.Pos.X + size.Width * 0.5,
                self.Pos.Y + size.Height * 0.5,
                size.Width * 0.5);
        }

        internal static Point MakeSlotPos(this IBiaNodeItem nodeItem, BiaNodeSlot slot)
        {
            var itemSize = nodeItem.Size;
            var itemPos = nodeItem.AlignedPos();

            var slotLocalPos = slot.MakePos(itemSize.Width, itemSize.Height);

            return new Point(itemPos.X + slotLocalPos.X, itemPos.Y + slotLocalPos.Y);
        }

        internal static BiaNodeSlot FindSlotFromPos(this IBiaNodeItem nodeItem, Point pos)
        {
            if (nodeItem.Layout.Slots == null)
                return null;

            var itemSize = nodeItem.Size;

            foreach (var slot in nodeItem.EnabledSlots())
            {
                var slotLocalPos = slot.MakePos(itemSize.Width, itemSize.Height);

                var d = (slotLocalPos, pos).DistanceSq();
                if (d <= Biaui.Internals.Constants.SlotMarkRadiusSq)
                    return slot;
            }

            return null;
        }

        internal static BiaNodeSlot FindSlotFromId(this IBiaNodeItem nodeItem, int slotId)
        {
            if (nodeItem.Layout.Slots == null)
                return null;

            if (nodeItem.Layout.Slots.TryGetValue(slotId, out var slot))
                return slot;

            return null;
        }

        internal static IEnumerable<BiaNodeSlot> EnabledSlots(this IBiaNodeItem nodeItem)
        {
            return nodeItem.InternalData().EnableSlots != null
                ? nodeItem.InternalData().EnableSlots
                : nodeItem.Layout.Slots.Values as IEnumerable<BiaNodeSlot>;
        }
    }
}