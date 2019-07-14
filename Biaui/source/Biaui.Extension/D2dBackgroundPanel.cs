using Biaui.Environment;
using SharpDX.Direct2D1;

namespace Biaui.Extension
{
    public class D2dBackgroundPanel : D2dControl.D2dControl, IBackgroundPanel
    {
        public void Invalidate()
        {
        }

        public override void Render(DeviceContext target)
        {
        }
    }
}