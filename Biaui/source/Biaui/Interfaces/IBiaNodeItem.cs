using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Biaui.Controls.NodeEditor;

namespace Biaui.Interfaces
{
    public interface IBiaNodeItem : IBiaHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        Size Size { get; set; }

        IReadOnlyDictionary<int, BiaNodeSlot> Slots { get; set; }

        object InternalData { get; set; }

        BiaNodePanelHitType HitType { get; }
        BiaNodePanelLayer Layer { get; }

        Func<BiaNodeSlot, Point> MakeSlotPos{ get; }        // nullでデフォルト動作になる

        bool CanMoveByDragging(CanMoveByDraggingArgs args);
    }

    public class CanMoveByDraggingArgs
    {
        public readonly IEnumerable<IBiaNodeItem> SelectedNodes;
        public object UserData;

        public CanMoveByDraggingArgs(IEnumerable<IBiaNodeItem> selectedNodes)
        {
            SelectedNodes = selectedNodes;
        }
    }

    public enum BiaNodePanelHitType
    {
        Rectangle,
        Circle,
        Visual
    }

    public enum BiaNodePanelLayer
    {
        Low = 0,
        Middle = 1,
        High = 2
    }
}