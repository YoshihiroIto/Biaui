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
                    Port1 = self.ItemPort1.FindPort(),
                    Port2 = self.ItemPort2.FindPort()
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