using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanel : Control
    {
        static BiaNodePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodePanel),
                new FrameworkPropertyMetadata(typeof(BiaNodePanel)));
        }

        internal void InvalidatePorts()
        {
            var ports = this.Descendants<BiaNodePanelPorts>().FirstOrDefault();

            ports?.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var ports = this.Descendants<BiaNodePanelPorts>().FirstOrDefault();

            ports?.UpdateMousePos(e.GetPosition(this));
            ports?.InvalidateVisual();
        }
    }
}