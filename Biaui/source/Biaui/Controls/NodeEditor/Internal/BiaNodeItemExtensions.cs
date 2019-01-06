using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class BiaNodeItemExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static InternalBiaNodeItemData InternalData(this IBiaNodeItem self)
        {
            InternalBiaNodeItemData internalData;

            if (self.InternalData == null)
            {
                internalData = new InternalBiaNodeItemData();
                self.InternalData = internalData;
            }
            else
            {
                internalData = (InternalBiaNodeItemData) self.InternalData;
            }

            return internalData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableRect MakeRect(this IBiaNodeItem self)
            => new ImmutableRect(self.Pos, self.Size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ImmutableCircle MakeCircle(this IBiaNodeItem self)
        {
            var size = self.Size;
            return new ImmutableCircle(
                self.Pos.X + size.Width * 0.5,
                self.Pos.Y + size.Height * 0.5,
                size.Width * 0.5);
        }

        internal static Point MakePortPos(this IBiaNodeItem nodeItem, BiaNodePort port)
        {
            var itemSize = nodeItem.Size;
            var itemPos = nodeItem.AlignedPos();

            var portLocalPos = port.MakePos(itemSize.Width, itemSize.Height);

            return new Point(itemPos.X + portLocalPos.X, itemPos.Y + portLocalPos.Y);
        }

        internal static BiaNodePort FindPortFromPos(this IBiaNodeItem nodeItem, Point pos)
        {
            if (nodeItem.Layout.Ports == null)
                return null;

            var itemSize = nodeItem.Size;

            foreach (var port in nodeItem.EnabledPorts())
            {
                var portLocalPos = port.MakePos(itemSize.Width, itemSize.Height);

                var d = (portLocalPos, pos).DistanceSq();
                if (d <= Biaui.Internals.Constants.PortMarkRadiusSq)
                    return port;
            }

            return null;
        }

        internal static BiaNodePort FindPortFromId(this IBiaNodeItem nodeItem, int portId)
        {
            if (nodeItem.Layout.Ports == null)
                return null;

            if (nodeItem.Layout.Ports.TryGetValue(portId, out var port))
                return port;

            return null;
        }

        internal static IEnumerable<BiaNodePort> EnabledPorts(this IBiaNodeItem nodeItem)
        {
            return nodeItem.InternalData().EnablePorts != null
                ? nodeItem.InternalData().EnablePorts
                : nodeItem.Layout.Ports.Values as IEnumerable<BiaNodePort>;
        }
    }
}