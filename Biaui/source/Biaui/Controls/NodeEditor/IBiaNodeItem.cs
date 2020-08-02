﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public interface IBiaNodeItem : IBiaHasPos, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }

        bool IsPreSelected { get; set; }

        bool IsMouseOver { get; set; }

        Size Size { get; set; }

        IReadOnlyDictionary<int, BiaNodeSlot>? Slots { get; set; }

        object? InternalData { get; set; }

        BiaNodePanelHitType HitType { get; }
        BiaNodePanelLayer Layer { get; }

        BiaNodePaneFlags Flags { get; }

        Func<BiaNodeSlot, Point>? MakeSlotPos { get; } // nullでデフォルト動作になる

        bool CanMoveByDragging(CanMoveByDraggingArgs args);
    }

    public class CanMoveByDraggingArgs
    {
        public readonly IEnumerable<IBiaNodeItem> SelectedNodes;
        public object? UserData;

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

    [Flags]
    public enum BiaNodePaneFlags
    {
        NoViewportCulling = 1 << 0,
        DesktopSpace = 1 << 1,

        None = 0
    }
}