using System.Runtime.CompilerServices;
using System.Windows;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodePortExtensions 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Point MakePos(this BiaNodePort port, double panelWidth, double panelHeight)
        {
            var i = ((int) port.Align << 2) | (int) port.Dir;

            var x = AlignPosTable[i * 2 + 0];
            var y = AlignPosTable[i * 2 + 1];

            return new Point(port.Offset.X + x * panelWidth, port.Offset.Y + y * panelHeight);
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