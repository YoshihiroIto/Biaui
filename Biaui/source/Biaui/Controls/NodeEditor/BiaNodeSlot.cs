using System;
using System.Windows;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeSlot
    {
        public int Id { get; set; }

        public Point Offset { get; set; }

        public BiaNodeSlotDir Dir { get; set; }

        public BiaNodeSlotAlign Align { get; set; }

        public ByteColor Color { get; set; } = ByteColor.White;

        public Func<Point /*スロット座標*/, Point /*マウス座標*/, bool>? TargetSlotHitChecker;
    }

    public enum BiaNodeSlotAlign
    {
        Start,
        Center,
        End
    }

    public enum BiaNodeSlotDir
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public static class BiaNodeSlotDirExtensions
    {
        public static bool IsVertical(this BiaNodeSlotDir self) =>
            self == BiaNodeSlotDir.Top ||
            self == BiaNodeSlotDir.Bottom;

        public static bool IsHorizontal(this BiaNodeSlotDir self) =>
            self == BiaNodeSlotDir.Left ||
            self == BiaNodeSlotDir.Right;
    }
}