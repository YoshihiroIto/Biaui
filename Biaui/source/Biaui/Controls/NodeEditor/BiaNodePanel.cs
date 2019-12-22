using System.Linq;
using System.Runtime.CompilerServices;
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

        public bool IsActive => DataContext != null;

        internal void InvalidateSlots()
        {
            var slots = this.Descendants<BiaNodePanelSlots>().FirstOrDefault();

            slots?.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var slots = this.Descendants<BiaNodePanelSlots>().FirstOrDefault();

            var pos = e.GetPosition(this);
            slots?.UpdateMousePos(Unsafe.As<Point, ImmutableVec2_double>(ref pos));
            slots?.InvalidateVisual();
        }
    }
}