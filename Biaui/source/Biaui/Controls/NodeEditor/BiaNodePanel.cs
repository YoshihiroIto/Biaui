using System.Windows;
using System.Windows.Controls;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanel : Control
    {
        static BiaNodePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodePanel),
                new FrameworkPropertyMetadata(typeof(BiaNodePanel)));
        }
    }
}