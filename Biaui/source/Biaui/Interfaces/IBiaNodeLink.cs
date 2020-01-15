using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Internals;

namespace Biaui.Interfaces
{
    public interface IBiaNodeLink : INotifyPropertyChanged
    {
        BiaNodeItemSlotIdPair ItemSlot1 { get; }

        BiaNodeItemSlotIdPair ItemSlot2 { get; }

        ByteColor Color { get; }

        BiaNodeLinkStyle Style { get; set; }

        object? InternalData { get; set; }

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
            var item1 = self.ItemSlot1.Item ?? throw new NullReferenceException();
            var item2 = self.ItemSlot2.Item ?? throw new NullReferenceException();

            return item1.IsSelected || item1.IsPreSelected || item1.IsMouseOver ||
                   item2.IsSelected || item2.IsPreSelected || item2.IsMouseOver;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(this IBiaNodeLink self)
        {
            self.InternalData = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTuple<ImmutableVec2_double, ImmutableVec2_double, ImmutableVec2_double, ImmutableVec2_double> MakeBezierCurve(this IBiaNodeLink self)
        {
            var item1 = self.ItemSlot1.Item ?? throw new NullReferenceException();
            var item2 = self.ItemSlot2.Item ?? throw new NullReferenceException();

            var pos1 = item1.MakeSlotPosDefault(self.InternalData().Slot1!);
            var pos2 = item2.MakeSlotPosDefault(self.InternalData().Slot2!);
            var pos1C = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, self.InternalData().Slot1!.Dir);
            var pos2C = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, self.InternalData().Slot2!.Dir);

            return (pos1, pos1C, pos2C, pos2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MakeBezierCurve(this IBiaNodeLink self, Span<ImmutableVec2_float> result)
        {
            var item1 = self.ItemSlot1.Item ?? throw new NullReferenceException();
            var item2 = self.ItemSlot2.Item ?? throw new NullReferenceException();

            var pos1 = item1.MakeSlotPosDefault(self.InternalData().Slot1!);
            var pos2 = item2.MakeSlotPosDefault(self.InternalData().Slot2!);
            var pos1C = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, self.InternalData().Slot1!.Dir);
            var pos2C = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, self.InternalData().Slot2!.Dir);

            result[0] = new ImmutableVec2_float((float)pos1.X,  (float)pos1.Y);
            result[1] = new ImmutableVec2_float((float)pos1C.X, (float)pos1C.Y);
            result[2] = new ImmutableVec2_float((float)pos2C.X, (float)pos2C.Y);
            result[3] = new ImmutableVec2_float((float)pos2.X,  (float)pos2.Y);
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