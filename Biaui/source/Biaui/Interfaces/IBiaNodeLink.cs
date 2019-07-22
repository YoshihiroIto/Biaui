using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        BiaNodeItemSlotIdPair ItemSlot1 { get; }

        BiaNodeItemSlotIdPair ItemSlot2 { get; }

        Color Color { get; }

        BiaNodeLinkStyle Style { get; set; }

        object InternalData { get; set; }

        bool IsVisible { get; }
    }

    public static class BiaNodeLinkExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLinked(this IBiaNodeLink self)
        {
            return self.InternalData().Slot1 != null &&
                   self.InternalData().Slot2 != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighlight(this IBiaNodeLink self)
        {
            var item1 = self.ItemSlot1.Item;
            var item2 = self.ItemSlot2.Item;

            return item1.IsSelected || item1.IsPreSelected || item1.IsMouseOver ||
                   item2.IsSelected || item2.IsPreSelected || item2.IsMouseOver;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(this IBiaNodeLink self)
        {
            self.InternalData = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTuple<Point, Point, Point, Point> MakeBezierCurve(this IBiaNodeLink self)
        {
            var item1 = self.ItemSlot1.Item;
            var item2 = self.ItemSlot2.Item;
            var pos1 = item1.MakeSlotPos(self.InternalData().Slot1);
            var pos2 = item2.MakeSlotPos(self.InternalData().Slot2);
            var pos1C = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, self.InternalData().Slot1.Dir);
            var pos2C = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, self.InternalData().Slot2.Dir);

            return (pos1, pos1C, pos2C, pos2);
        }
    }

    [Flags]
    public enum BiaNodeLinkStyle
    {
        None,

        //
        DashedLine = 1 << 0,
        Arrow = 1 << 1
    }
}