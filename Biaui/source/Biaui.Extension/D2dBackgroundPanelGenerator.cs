using Biaui.Controls.NodeEditor;
using Biaui.Environment;

namespace Biaui.Extension
{
    public class D2dBackgroundPanelGenerator : IBackgroundPanelGenerator
    {
        public IBackgroundPanel Generate(BiaNodeEditor parent)
            => new D2dBackgroundPanel(parent);

        public void Initialize()
        {
            D2dControl.D2dControl.Initialize();
        }

        public void Destroy()
        {
            D2dControl.D2dControl.Destroy();
        }
    }
}