using System.Runtime.CompilerServices;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeLinkExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static InternalBiaNodeLinkData InternalData(this IBiaNodeLink self)
        {
            InternalBiaNodeLinkData internalData;

            if (self.InternalData is null)
            {
                internalData = new InternalBiaNodeLinkData
                {
                    Slot1 = self.SourceSlot.FindSlot(),
                    Slot2 = self.TargetSlot.FindSlot()
                };
                self.InternalData = internalData;
            }
            else
            {
                internalData = (InternalBiaNodeLinkData) self.InternalData;
            }

            return internalData;
        }
    }
}