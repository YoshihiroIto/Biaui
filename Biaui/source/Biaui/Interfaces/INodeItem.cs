using System.ComponentModel;
using System.Windows;
using Biaui.Internals;

namespace Biaui.Interfaces
{
    public interface INodeItem : IHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        bool IsPreSelected { get; set; }

        Size Size { get; set; }

        bool IsRequireVisualTest { get; }
    }

    internal static class NodeItemExtensions
    {
        internal static bool IntersectsWith(this INodeItem self, in ImmutableRect rect)
        {
            var pos = self.Pos;
            var size = self.Size;

            return rect.IntersectsWith(pos.X, pos.Y, pos.X + size.Width, pos.Y + size.Height);
        }

        internal static ImmutableRect MakeRect(this INodeItem self)
            => new ImmutableRect(self.Pos, self.Size);
    }
}