using Biaui.Interfaces;
using Jewelry.Memory;

namespace Biaui.Controls.NodeEditor
{
    public interface IBiaNodeSlotEnabledChecker
    {
        bool IsEnableSlot(in BiaNodeItemSlotIdPair slot);
        void Check(IBiaNodeItem target, in BiaNodeSlotEnabledCheckerArgs args, ref TempBuffer<int> result);
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