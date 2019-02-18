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

            if (self.InternalData == null)
            {
                internalData = new InternalBiaNodeLinkData
                {
                    Slot1 = self.ItemSlot1.FindSlot(),
                    Slot2 = self.ItemSlot2.FindSlot()
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