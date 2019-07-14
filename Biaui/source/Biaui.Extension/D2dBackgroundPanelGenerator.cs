using Biaui.Controls.NodeEditor;
using Biaui.Environment;

namespace Biaui.Extension
{
    public class D2dBackgroundPanelGenerator
    {
        public IBackgroundPanel Generate(BiaNodeEditor parent)
            => new D2dBackgroundPanel(parent);
    }
}