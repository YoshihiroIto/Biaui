using System;
using System.Collections.Generic;
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

        private Point _mousePoint;

        internal void UpdateMousePos(Point point)
        {
            _mousePoint = point;
        }

        private static readonly Dictionary<Color, (StreamGeometry Geom, StreamGeometryContext Ctx)> _ellipses =
            new Dictionary<Color, (StreamGeometry, StreamGeometryContext)>();

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var nodeItem = (IBiaNodeItem) DataContext;
            var isMouseOverNode = nodeItem.IsMouseOver;

            foreach (var slot in nodeItem.EnabledSlots())
            {
                var slotPos = slot.MakePos(nodeItem.Size.Width, nodeItem.Size.Height);

                var r = Biaui.Internals.Constants.SlotMarkRadius;

                // パネルがマウスオーバー時は、ポート自体のマウス位置見て半径を作る
                if (isMouseOverNode)
                    if ((slotPos, _mousePoint).DistanceSq() <= Biaui.Internals.Constants.SlotMarkRadiusSq)
                        r = Biaui.Internals.Constants.SlotMarkRadius_Highlight;

                var color = slot.Color;

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

                curve.Ctx.DrawEllipse(slotPos, r, r, true, true);
            }

            var pen = Caches.GetPen(Colors.Black, 2);

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