using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanel : Control //Thumb
    {
        public BiaNodePanel()
        {
            //DragDelta += OnDragDelta;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var i = (INodeItem)DataContext;

            i.Pos = new Point(i.Pos.X + e.HorizontalChange, i.Pos.Y + e.VerticalChange);
        }
    }
}
