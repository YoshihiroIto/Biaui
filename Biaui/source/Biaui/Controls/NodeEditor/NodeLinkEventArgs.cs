using System;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    /// <summary>
    /// ノード接続開始時
    /// </summary>
    public class NodeLinkStartingEventArgs : EventArgs
    {
        public IBiaNodeItem SourceNodeItem { get; }
        public int SourcePortId { get; }

        internal NodeLinkStartingEventArgs(IBiaNodeItem sourceNodeItem, int sourcePortId)
        {
            SourceNodeItem = sourceNodeItem;
            SourcePortId = sourcePortId;
        }
    }

    /// <summary>
    /// ノード接続中
    /// </summary>
    public class NodeLinkConnectingEventArgs : EventArgs
    {
        public IBiaNodeItem SourceNodeItem { get; }
        public int SourcePortId { get; }

        public IBiaNodeItem TargetNodeItem { get; }
        public int TargetPortId { get; }

        internal NodeLinkConnectingEventArgs(
            IBiaNodeItem sourceNodeItem, int sourcePortId,
            IBiaNodeItem targetNodeItem, int targetPortId)
        {
            SourceNodeItem = sourceNodeItem;
            SourcePortId = sourcePortId;
            TargetNodeItem = targetNodeItem;
            TargetPortId = targetPortId;
        }
    }

    /// <summary>
    /// ノード接続終了
    /// </summary>
    public class NodeLinkCompletedEventArgs : EventArgs
    {
        public IBiaNodeItem SourceNodeItem { get; }
        public int SourcePortId { get; }

        public IBiaNodeItem TargetNodeItem { get; }
        public int TargetPortId { get; }

        internal NodeLinkCompletedEventArgs(
            IBiaNodeItem sourceNodeItem, int sourcePortId,
            IBiaNodeItem targetNodeItem, int targetPortId)
        {
            SourceNodeItem = sourceNodeItem;
            SourcePortId = sourcePortId;
            TargetNodeItem = targetNodeItem;
            TargetPortId = targetPortId;
        }
    }
}
