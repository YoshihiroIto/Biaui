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
    public class BiaNodePanelPorts : Border
    {
        static BiaNodePanelPorts()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodePanelPorts),
                new FrameworkPropertyMetadata(typeof(BiaNodePanelPorts)));
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

            foreach (var port in nodeItem.EnabledPorts())
            {
                var portPos = port.MakePos(ActualWidth, ActualHeight);

                var r = Biaui.Internals.Constants.PortMarkRadius;

                // パネルがマウスオーバー時は、ポート自体のマウス位置見て半径を作る
                if (isMouseOverNode)
                    if ((portPos, _mousePoint).DistanceSq() <= Biaui.Internals.Constants.PortMarkRadiusSq)
                        r = Biaui.Internals.Constants.PortMarkRadius_Highlight;

                var color = port.Color;

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

                curve.Ctx.DrawEllipse(portPos, r, r, true, true);
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