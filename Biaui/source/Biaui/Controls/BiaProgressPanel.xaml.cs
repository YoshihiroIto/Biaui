using System.Windows;
using System.Windows.Controls;

namespace Biaui.Controls;

public class BiaProgressPanel : Control
{
    static BiaProgressPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaProgressPanel),
            new FrameworkPropertyMetadata(typeof(BiaProgressPanel)));
    }
}