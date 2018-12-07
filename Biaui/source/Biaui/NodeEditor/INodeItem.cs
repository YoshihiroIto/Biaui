using System.ComponentModel;
using System.Windows;

namespace Biaui.NodeEditor
{
    public interface INodeItem : INotifyPropertyChanged
    {
        Point Pos { get; set; }
    }

    public static class NodeItemExtensions
    {
        public static bool IntersectsWith(this INodeItem self, double width, double height, Rect rect)
        {
            var pos = self.Pos;

            var childRect = new Rect(pos.X, pos.Y, width, height);

            return rect.IntersectsWith(childRect);
        }
    }
}