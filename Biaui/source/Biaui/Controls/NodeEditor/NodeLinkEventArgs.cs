using System;

namespace Biaui.Controls.NodeEditor
{
    /// <summary>
    /// ノード接続開始時
    /// </summary>
    public class NodeLinkStartingEventArgs : EventArgs
    {
        public readonly BiaNodeItemPortIdPair Source;

        internal NodeLinkStartingEventArgs(in BiaNodeItemPortIdPair source)
        {
            Source = source;
        }
    }

    /// <summary>
    /// ノード接続終了
    /// </summary>
    public class NodeLinkCompletedEventArgs : EventArgs
    {
        public readonly BiaNodeItemPortIdPair Source;
        public readonly BiaNodeItemPortIdPair Target;

        internal NodeLinkCompletedEventArgs(
            in BiaNodeItemPortIdPair source,
            in BiaNodeItemPortIdPair target)
        {
            Source = source;
            Target = target;
        }
    }
}
