using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanelSlots : Border
    {
        static BiaNodePanelSlots()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodePanelSlots),
                new FrameworkPropertyMetadata(typeof(BiaNodePanelSlots)));
        }

        private ImmutableVec2_double _mousePoint;

        internal void UpdateMousePos(in ImmutableVec2_double point)
        {
            _mousePoint = point;
        }

        private static readonly Dictionary<ByteColor, (StreamGeometry Geom, StreamGeometryContext Ctx)> _ellipses =
            new Dictionary<ByteColor, (StreamGeometry, StreamGeometryContext)>();

        private BiaNodeEditor? _parent;

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var nodeItem = (IBiaNodeItem) DataContext;
            if (nodeItem is null)
                return;

            var rounder = new LayoutRounder(this);
            
            if (_parent is null)
                _parent = this.GetParent<BiaNodeEditor>() ?? throw new NullReferenceException();

            var canConnectLink = _parent.CanConnectLink;
            var isMouseOverNode = nodeItem.IsMouseOver && canConnectLink;

            var baseRadius = Biaui.Internals.Constants.SlotMarkRadius(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));
            var baseRadiusSq = Biaui.Internals.Constants.SlotMarkRadiusSq(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));
            var baseHighlightRadius = Biaui.Internals.Constants.SlotMarkRadius_Highlight(nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace));

            Pen pen;

            if (nodeItem.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
            {
                var invScale = 1d / this.CalcCompositeRenderScale();
                baseRadius *= invScale;
                baseRadiusSq *= invScale * invScale;     // 半径の二乗なのでスケールも２度掛ける
                baseHighlightRadius *= invScale;
                
                pen = Caches.GetPen(ByteColor.Black, rounder.RoundLayoutValue(2d * invScale));
            }
            else
            {
                pen = Caches.GetPen(ByteColor.Black, rounder.RoundLayoutValue(2d));
            }
            
            foreach (var slot in nodeItem.EnabledSlots())
            {
                var slotPos = slot.MakePos(nodeItem.Size.Width, nodeItem.Size.Height);

                var r = baseRadius;

                // ポート自体が有効でパネルがマウスオーバー時は、ポート自体のマウス位置見て半径を作る
                if (isMouseOverNode)
                {
                    if (_parent.NodeSlotEnabledChecker != null)
                    {
                        var slotData = new BiaNodeItemSlotIdPair(nodeItem, slot.Id);

                        if (_parent.NodeSlotEnabledChecker.IsEnableSlot(slotData))
                            if ((slotPos, _mousePoint).DistanceSq() <= baseRadiusSq)
                                r = baseHighlightRadius;
                    }
                }

                var color = canConnectLink ? slot.Color : ByteColor.DimGray;

                if (_ellipses.TryGetValue(color, out var curve) == false)
                {
                    var geom = new StreamGeometry
                    {
                        FillRule = FillRule.Nonzero
                    };
                    var ctx = geom.Open();

                    curve = (geom, ctx);
                    _ellipses.Add(color, curve);
                }

                curve.Ctx.DrawEllipse(Unsafe.As<ImmutableVec2_double, Point>(ref slotPos), r, r, true, true);
            }

            foreach (var c in _ellipses)
            {
                var brush = Caches.GetSolidColorBrush(c.Key);

                ((IDisposable) c.Value.Ctx).Dispose();
                dc.DrawGeometry(brush, pen, c.Value.Geom);
            }

            _ellipses.Clear();
        }
    }
}