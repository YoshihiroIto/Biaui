using System.Collections.Generic;
using System.Windows;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class InternalBiaNodeItemData
    {
        internal readonly HashSet<BiaNodeSlot> EnableSlots = new HashSet<BiaNodeSlot>();

        internal Style? Style;
    }
}