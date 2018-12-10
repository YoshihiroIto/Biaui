using System.ComponentModel;
using System.Windows;

namespace Biaui.NodeEditor
{
    public interface INodeItem : INotifyPropertyChanged
    {
        Point Pos { get; set; }
        Size Size { get; set; }
    }

    public static class NodeItemExtensions
    {
        public static bool IntersectsWith(this INodeItem self, Rect rect)
        {
            var pos = self.Pos;
            var size = self.Size;

            var childRect = new Rect(pos.X, pos.Y, size.Width, size.Height);

            return rect.IntersectsWith(childRect);
        }
    }
}