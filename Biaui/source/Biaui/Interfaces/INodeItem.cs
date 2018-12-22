using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Internals;

namespace Biaui.Interfaces
{
    public interface INodeItem : IHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        bool IsRequireVisualTest { get; }

        Size Size { get; set; }

        BiaNodePortLayout Layout { get; }
    }

    public static class NodeItemExtensions
    {
        public static ImmutableRect MakeRect(this INodeItem self)
            => new ImmutableRect(self.Pos, self.Size);

        public static (Point Pos, BiaNodePortDir Dir) MakePortPos(this INodeItem nodeItem, string portId)
        {
            var port = nodeItem.Layout.FindPort(portId);

            if (port == null)
                throw new NotSupportedException();

            return MakePortPos(nodeItem, port);
        }

        public static (Point Pos, BiaNodePortDir Dir) MakePortPos(this INodeItem nodeItem, BiaNodePort port)
        {
            var itemSize = nodeItem.Size;
            var itemPos = nodeItem.Pos;

            var alignPos = NodeEditorHelper.MakeAlignPos(port);

            return (
                new Point(
                    itemPos.X + port.Offset.X + alignPos.X * itemSize.Width,
                    itemPos.Y + port.Offset.Y + alignPos.Y * itemSize.Height),
                port.Dir);
        }

        public static BiaNodePort FindNodePort(this INodeItem nodeItem, Point pos)
        {
            if (nodeItem.Layout.Ports == null)
                return null;

            foreach (var port in nodeItem.Layout.Ports.Values)
            {
                var size = nodeItem.Size;
                var portAlignPos = NodeEditorHelper.MakeAlignPos(port);
                var portPos = new Point(portAlignPos.X *size.Width + port.Offset.X, portAlignPos.Y * size.Height+ port.Offset.Y);

                var d = (portPos, pos).DistanceSq();
                if (d <= Internals.Constants.NodePanelPortMarkRadiusSq)
                    return port;
            }

            return null;
        }
    }

    public class BiaNodePortLayout
    {
        public Dictionary<string, BiaNodePort> Ports { get; set; }

        public BiaNodePort FindPort(string portId)
        {
            if (Ports == null)
                return null;

            if (Ports.TryGetValue(portId, out var port))
                return port;

            return null;
        }
    }

    public enum BiaNodePortDir
    {
        Left,
        Top,
        Right,
        Bottom,

        //
        None
    }

    public interface ILinkItem : INotifyPropertyChanged
    {
        INodeItem Item1 { get; }

        string Item1PortId { get; }

        INodeItem Item2 { get; }

        string Item2PortId { get; }

        object InternalData { get; set; }
    }
}