using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;
using Biaui.Internals;

namespace Biaui.Interfaces
{
    public interface INodeItem : IHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsRequireVisualTest { get; }

        Size Size { get; set; }

        BiaNodePortLayout Layout { get; }
    }

    public static class NodeItemExtensions
    {
        public static ImmutableRect MakeRect(this INodeItem self)
            => new ImmutableRect(self.Pos, self.Size);

        public static (Point, BiaNodePortDir) MakePortPos(this INodeItem nodeItem, string portId)
        {
            var port = nodeItem.Layout.FindPort(portId);

            if (port == null)
                throw new NotSupportedException();

            var alignPos = NodeEditorHelper.MakeAlignPos(port.Dir, port.Align, nodeItem.Size.Width, nodeItem.Size.Height);

            return (
                new Point(
                    nodeItem.Pos.X + port.Offset.X + alignPos.X,
                    nodeItem.Pos.Y + port.Offset.Y + alignPos.Y),
                port.Dir);
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
        INodeItem Item0 { get; }

        string Item0PortId { get; }

        INodeItem Item1 { get; }

        string Item1PortId { get; }
    }
}