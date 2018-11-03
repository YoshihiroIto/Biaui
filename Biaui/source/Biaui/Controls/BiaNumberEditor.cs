using System.Windows;
using System.Windows.Controls;

namespace Biaui.Controls
{
    public class BiaNumberEditor : Control
    {
        static BiaNumberEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNumberEditor),
                new FrameworkPropertyMetadata(typeof(BiaNumberEditor)));
        }
    }
}