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

        internal void InvalidateSlots()
        {
            var slots = this.Descendants<BiaNodePanelSlots>().FirstOrDefault();

            slots?.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var slots = this.Descendants<BiaNodePanelSlots>().FirstOrDefault();

            slots?.UpdateMousePos(e.GetPosition(this));
            slots?.InvalidateVisual();
        }
    }
}