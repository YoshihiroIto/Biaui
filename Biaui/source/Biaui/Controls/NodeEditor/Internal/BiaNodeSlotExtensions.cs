using System.Runtime.CompilerServices;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeSlotExtensions
    {
        internal static bool HitCheck(this BiaNodeSlot slot, in ImmutableVec2 slotPos, in ImmutableVec2 mousePos)
        {
            if (slot.TargetSlotHitChecker != null)
            {
                if (slot.TargetSlotHitChecker(slotPos.ToPoint(), mousePos.ToPoint()) == false)
                    return false;
            }
            else if ((slotPos, mousePos).DistanceSq() > Biaui.Internals.Constants.SlotMarkRadiusSq)
                return false;

            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableVec2 MakePos(this BiaNodeSlot slot, double panelWidth, double panelHeight)
        {
            var i = ((int) slot.Align << 2) | (int) slot.Dir;

            var x = AlignPosTable[i * 2 + 0];
            var y = AlignPosTable[i * 2 + 1];

            return new ImmutableVec2(slot.Offset.X + x * panelWidth, slot.Offset.Y + y * panelHeight);
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