using System.Collections.Generic;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public interface IBiaNodePortEnabledChecker
    {
        IEnumerable<int> Check(IBiaNodeItem target, in BiaNodePortEnabledCheckerArgs args);
    }

    public enum BiaNodePortEnableTiming
    {
        Default,
        ConnectionStarting
    }

    public readonly struct BiaNodePortEnabledCheckerArgs
    {
        public readonly BiaNodePortEnableTiming Timing;
        public readonly IBiaNodeItem SourceItem;
        public readonly int SourceItemPortId;

        internal BiaNodePortEnabledCheckerArgs(
            BiaNodePortEnableTiming timing,
            IBiaNodeItem sourceItem,
            int sourceItemPortId
        )
        {
            Timing = timing;
            SourceItem = sourceItem;
            SourceItemPortId = sourceItemPortId;
        }
    }
}