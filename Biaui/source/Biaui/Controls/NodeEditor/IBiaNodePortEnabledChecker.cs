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
        public readonly BiaNodeItemPortIdPair Source;

        internal BiaNodePortEnabledCheckerArgs(
            BiaNodePortEnableTiming timing,
            BiaNodeItemPortIdPair source
        )
        {
            Timing = timing;
            Source = source;
        }
    }
}