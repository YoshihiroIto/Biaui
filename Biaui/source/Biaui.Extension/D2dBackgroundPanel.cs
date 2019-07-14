using Biaui.Controls.NodeEditor;
using Biaui.Environment;
using SharpDX.Direct2D1;

namespace Biaui.Extension
{
    public class D2dBackgroundPanel : D2dControl.D2dControl, IBackgroundPanel
    {
        private readonly BiaNodeEditor _parent;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            _parent = parent;
        }

        public void Invalidate()
        {
        }

        public override void Render(DeviceContext target)
        {
        }
    }
}