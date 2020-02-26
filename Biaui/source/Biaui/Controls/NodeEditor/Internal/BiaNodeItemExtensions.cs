using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        internal static ImmutableRect_double MakeRect(this IBiaNodeItem self)
            => new ImmutableRect_double(self.Pos, self.Size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableCircle_double MakeCircle(this IBiaNodeItem self)
        {
            var size = self.Size;
            return new ImmutableCircle_double(
                self.Pos.X + size.Width * 0.5,
                self.Pos.Y + size.Height * 0.5,
                size.Width * 0.5);
        }

        internal static ImmutableVec2_double MakeSlotPosDefault(this IBiaNodeItem nodeItem, BiaNodeSlot slot)
        {
            Debug.Assert(slot != null);

            if (nodeItem.MakeSlotPos != null)
            {
                var p = nodeItem.MakeSlotPos(slot);
                return Unsafe.As<Point, ImmutableVec2_double>(ref p);
            }

            var itemSize = nodeItem.Size;
            var itemPos = nodeItem.AlignPos();

            var slotLocalPos = slot.MakePos(itemSize.Width, itemSize.Height);

            return new ImmutableVec2_double(itemPos.X + slotLocalPos.X, itemPos.Y + slotLocalPos.Y);
        }

        internal static BiaNodeSlot? FindSlotFromPos(
            this IBiaNodeItem nodeItem, in ImmutableVec2_double pos, FrameworkElement control)
        {
            if (nodeItem.Slots == null)
                return null;

            var itemSize = nodeItem.Size;

            var slotMarkRadiusSq = control.MakeSlotMarkRadiusSq(nodeItem);
            
            foreach (var slot in nodeItem.EnabledSlots())
            {
                var slotLocalPos = slot.MakePos(itemSize.Width, itemSize.Height);

                var d = (slotLocalPos, pos).DistanceSq();
                if (d <= slotMarkRadiusSq)
                    return slot;
            }

            return null;
        }

        internal static BiaNodeSlot? FindSlotFromId(this IBiaNodeItem nodeItem, int slotId)
        {
            if (nodeItem.Slots == null)
                return null;

            if (nodeItem.Slots.TryGetValue(slotId, out var slot))
                return slot;

            return null;
        }

        internal static IEnumerable<BiaNodeSlot> EnabledSlots(this IBiaNodeItem nodeItem)
        {
            return nodeItem.InternalData().EnableSlots ??
                   nodeItem.Slots?.Values ??
                   Enumerable.Empty<BiaNodeSlot>();
        }
    }
}