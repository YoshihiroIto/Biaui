using System;
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

        public static (Point, BiaNodePortDir) MakePortPos(this INodeItem nodeItem, object portId)
        {
            var (port, dir) = nodeItem.Layout.FindPort(portId);

            if (port == null)
                throw new NotSupportedException();

            var alignPos = NodeEditorHelper.MakeAlignPos(dir, port.Align, nodeItem.Size.Width, nodeItem.Size.Height);

            return (
                new Point(
                    nodeItem.Pos.X + port.Offset.X + alignPos.X,
                    nodeItem.Pos.Y + port.Offset.Y + alignPos.Y),
                dir);
        }
    }

    public class BiaNodePortLayout
    {
        public BiaNodePort[] LeftPorts { get; set; }

        public BiaNodePort[] TopPorts { get; set; }

        public BiaNodePort[] RightPorts { get; set; }

        public BiaNodePort[] BottomPorts { get; set; }

        public (BiaNodePort, BiaNodePortDir) FindPort(object portId)
        {
            if (LeftPorts != null)
                foreach (var p in LeftPorts)
                    if (p.Id == portId)
                        return (p, BiaNodePortDir.Left);

            if (TopPorts != null)
                foreach (var p in TopPorts)
                    if (p.Id == portId)
                        return (p, BiaNodePortDir.Top);

            if (RightPorts != null)
                foreach (var p in RightPorts)
                    if (p.Id == portId)
                        return (p, BiaNodePortDir.Right);

            if (BottomPorts != null)
                foreach (var p in BottomPorts)
                    if (p.Id == portId)
                        return (p, BiaNodePortDir.Bottom);

            return (null, BiaNodePortDir.None);
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

        object Item0PortId { get; }

        INodeItem Item1 { get; }

        object Item1PortId { get; }
    }
}