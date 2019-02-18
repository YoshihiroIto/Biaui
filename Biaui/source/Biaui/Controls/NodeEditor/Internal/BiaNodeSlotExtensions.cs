using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeSlotExtensions 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Point MakePos(this BiaNodeSlot slot, double panelWidth, double panelHeight)
        {
            var i = ((int) slot.Align << 2) | (int) slot.Dir;

            var x = AlignPosTable[i * 2 + 0];
            var y = AlignPosTable[i * 2 + 1];

            return new Point(slot.Offset.X + x * panelWidth, slot.Offset.Y + y * panelHeight);
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