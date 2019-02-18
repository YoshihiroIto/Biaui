using System;

namespace Biaui.Controls.NodeEditor
{
    /// <summary>
    /// ノード接続開始時
    /// </summary>
    public class NodeLinkStartingEventArgs : EventArgs
    {
        public readonly BiaNodeItemSlotIdPair Source;

        internal NodeLinkStartingEventArgs(in BiaNodeItemSlotIdPair source)
        {
            Source = source;
        }
    }

    /// <summary>
    /// ノード接続終了
    /// </summary>
    public class NodeLinkCompletedEventArgs : EventArgs
    {
        public readonly BiaNodeItemSlotIdPair Source;
        public readonly BiaNodeItemSlotIdPair Target;

        internal NodeLinkCompletedEventArgs(
            in BiaNodeItemSlotIdPair source,
            in BiaNodeItemSlotIdPair target)
        {
            Source = source;
            Target = target;
        }
    }
}
