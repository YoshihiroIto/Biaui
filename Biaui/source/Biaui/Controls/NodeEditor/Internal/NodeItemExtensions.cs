using System.Windows;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class NodeItemExtensions
    {
        internal static ImmutableRect MakeRect(this INodeItem self)
            => new ImmutableRect(self.Pos, self.Size);

        internal static Point MakePortPos(this INodeItem nodeItem, BiaNodePort port)
        {
            var itemSize = nodeItem.Size;
            var itemPos = nodeItem.Pos;

            var portLocalPos = port.MakePos(itemSize.Width, itemSize.Height);

            return new Point(itemPos.X + portLocalPos.X, itemPos.Y + portLocalPos.Y);
        }

        internal static BiaNodePort FindPortFromPos(this INodeItem nodeItem, Point pos)
        {
            if (nodeItem.Layout.Ports == null)
                return null;

            var itemSize = nodeItem.Size;

            foreach (var port in nodeItem.Layout.Ports.Values)
            {
                var portLocalPos = port.MakePos(itemSize.Width, itemSize.Height);

                var d = (portLocalPos, pos).DistanceSq();
                if (d <= Biaui.Internals.Constants.PortMarkRadiusSq)
                    return port;
            }

            return null;
        }

        internal static BiaNodePort FindPortFromId(this INodeItem nodeItem, string portId)
        {
            if (nodeItem.Layout.Ports == null)
                return null;

            if (nodeItem.Layout.Ports.TryGetValue(portId, out var port))
                return port;

            return null;
        }
    }
}