using System.Collections.Generic;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public interface IBiaNodeSlotEnabledChecker
    {
        IEnumerable<int> Check(IBiaNodeItem target, in BiaNodeSlotEnabledCheckerArgs args);
    }

    public enum BiaNodeSlotEnableTiming
    {
        Default,
        ConnectionStarting
    }

    public readonly struct BiaNodeSlotEnabledCheckerArgs
    {
        public readonly BiaNodeSlotEnableTiming Timing;
        public readonly BiaNodeItemSlotIdPair Source;

        internal BiaNodeSlotEnabledCheckerArgs(
            BiaNodeSlotEnableTiming timing,
            in BiaNodeItemSlotIdPair source
        )
        {
            Timing = timing;
            Source = source;
        }
    }
}