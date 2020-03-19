using System;
using System.Runtime.CompilerServices;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeSlotExtensions
    {
        internal static bool HitCheck(
            this BiaNodeSlot slot,
            double invScale, IBiaNodeItem nodeItem,
            in ImmutableVec2_double slotPos, in ImmutableVec2_double mousePos)
        {
            if (slot.TargetSlotHitChecker != null)
                if (slot.TargetSlotHitChecker(slotPos.ToPoint(), mousePos.ToPoint()) == false)
                    return false;

            var slotMarkRadiusSq = Biaui.Internals.Constants.SlotMarkRadiusSq(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));

            if (nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
                slotMarkRadiusSq *= invScale * invScale;

            return (slotPos, mousePos).DistanceSq() <= slotMarkRadiusSq;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableVec2_double MakePos(this BiaNodeSlot slot, double panelWidth, double panelHeight)
        {
            var i = ((int) slot.Align << 2) | (int) slot.Dir;

            var x = AlignPosTable[i * 2 + 0];
            var y = AlignPosTable[i * 2 + 1];

            return new ImmutableVec2_double(slot.Offset.X + x * panelWidth, slot.Offset.Y + y * panelHeight);
        }

        private static readonly double[] AlignPosTable =
        {
            //
            0.0, 0.0,
            0.0, 0.0,
            1.0, 0.0,
            0.0, 1.0,
            //
            0.0, 0.5,
            0.5, 0.0,
            1.0, 0.5,
            0.5, 1.0,
            //
            0.0, 1.0,
            1.0, 0.0,
            1.0, 1.0,
            1.0, 1.0
        };
    }
}