using System.Collections.Generic;
using System.Windows;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class InternalBiaNodeItemData
    {
        internal readonly HashSet<BiaNodePort> EnablePorts = new HashSet<BiaNodePort>();

        internal Style Style;
    }
}