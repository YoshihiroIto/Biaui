using System;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    /// <summary>
    /// ノード接続開始時
    /// </summary>
    public class NodeLinkStartingEventArgs : EventArgs
    {
        public bool IsCancel { get; set; }

        public IBiaNodeItem SourceNodeItem { get; }
        public string SourcePortId { get; }

        internal NodeLinkStartingEventArgs(IBiaNodeItem sourceNodeItem, string sourcePortId)
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
        public bool CanConnect { get; set; }

        public IBiaNodeItem SourceNodeItem { get; }
        public string SourcePortId { get; }

        public IBiaNodeItem TargetNodeItem { get; }
        public string TargetPortId { get; }

        internal NodeLinkConnectingEventArgs(
            IBiaNodeItem sourceNodeItem, string sourcePortId,
            IBiaNodeItem targetNodeItem, string targetPortId)
        {
            CanConnect = true;

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
        public string SourcePortId { get; }

        public IBiaNodeItem TargetNodeItem { get; }
        public string TargetPortId { get; }

        internal NodeLinkCompletedEventArgs(
            IBiaNodeItem sourceNodeItem, string sourcePortId,
            IBiaNodeItem targetNodeItem, string targetPortId)
        {
            SourceNodeItem = sourceNodeItem;
            SourcePortId = sourcePortId;
            TargetNodeItem = targetNodeItem;
            TargetPortId = targetPortId;
        }
    }
}
