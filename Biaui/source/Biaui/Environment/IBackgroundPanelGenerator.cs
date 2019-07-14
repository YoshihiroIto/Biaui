using Biaui.Controls.NodeEditor;

namespace Biaui.Environment
{
    public interface IBackgroundPanelGenerator
    {
        IBackgroundPanel Generate(BiaNodeEditor parent);
    }
}