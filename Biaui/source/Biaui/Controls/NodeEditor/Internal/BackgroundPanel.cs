using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor.Internal.NodeLinkGeomMaker;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BackgroundPanel : Canvas
    {
        private readonly BiaNodeEditor _parent;

        private const double LineWidth = 3;
        private const double ArrowSize = 20;

        static BackgroundPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
        }

        internal BackgroundPanel(BiaNodeEditor parent, MouseOperator mouseOperator)
        {
            _parent = parent;

            _parent.TranslateTransform.Changed += (_, __) => InvalidateVisual();
            _parent.ScaleTransform.Changed += (_, __) => InvalidateVisual();
            _parent.NodeItemMoved += (_, __) => InvalidateVisual();
            _parent.LinksSourceChanging += (_, __) => InvalidateVisual();
            _parent.LinkChanged += (_, __) => InvalidateVisual();

            mouseOperator.PreMouseLeftButtonUp += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            //DrawGrid(dc);

            DrawNodeLink(dc);
        }

#if false
        private readonly StreamGeometry _gridGeom = new StreamGeometry();

        private void DrawGrid(DrawingContext dc)
        {
            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _parent.ScaleTransform.ScaleX;
            var tx = _parent.TranslateTransform.X;
            var ty = _parent.TranslateTransform.Y;

            var bx = FrameworkElementHelper.RoundLayoutValue(ActualWidth);
            var by = FrameworkElementHelper.RoundLayoutValue(ActualHeight);

            _gridGeom.Clear();

            var geomCtx = _gridGeom.Open();
            {
                for (var h = 0;; ++h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x < 0) continue;

                    if (x > ActualWidth) break;

                    geomCtx.BeginFigure(new Point(x, 0), false, false);
                    geomCtx.LineTo(new Point(x, by), true, false);
                }

                for (var h = 0;; --h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x > ActualWidth) continue;

                    if (x < 0) break;

                    geomCtx.BeginFigure(new Point(x, 0), false, false);
                    geomCtx.LineTo(new Point(x, by), true, false);
                }

                for (var v = 0;; ++v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y < 0) continue;

                    if (y > ActualHeight) break;

                    geomCtx.BeginFigure(new Point(0, y), false, false);
                    geomCtx.LineTo(new Point(bx, y), true, false);
                }

                for (var v = 0;; --v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y > ActualHeight) continue;

                    if (y < 0) break;

                    geomCtx.BeginFigure(new Point(0, y), false, false);
                    geomCtx.LineTo(new Point(bx, y), true, false);
                }
            }
            ((IDisposable) geomCtx).Dispose();
            dc.DrawGeometry(null, p, _gridGeom);
        }
#endif

        private static readonly Dictionary<(Color Color, BiaNodeLinkStyle Style, bool IsHightlight), (StreamGeometry Geom, StreamGeometryContext Ctx)> _curves
            = new Dictionary<(Color, BiaNodeLinkStyle, bool), (StreamGeometry, StreamGeometryContext)>();

        private static readonly IReadOnlyDictionary<BiaNodeEditorNodeLinkStyle, INodeLinkGeomMaker> NodeLinkGeomMakers =
            new Dictionary<BiaNodeEditorNodeLinkStyle, INodeLinkGeomMaker>()
            {
                {BiaNodeEditorNodeLinkStyle.AxisAlign, new AxisAlignStyleNodeLinkGeomMaker(ArrowSize)},
                {BiaNodeEditorNodeLinkStyle.BezierCurve, new BezierCurveStyleMakeNodeGeomLink(ArrowSize)}
            };

        private void DrawNodeLink(DrawingContext dc)
        {
            if (_parent.LinksSource == null)
                return;

            var inflate = ArrowSize * _parent.ScaleTransform.ScaleX;
            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);
            var lineCullingRect = new ImmutableRect(
                viewport.X - inflate,
                viewport.Y - inflate,
                viewport.Width + inflate * 2,
                viewport.Height + inflate * 2
            );

            var alpha = _parent.IsNodeSlotDragging
                ? 0.2
                : 1.0;

            var backgroundColor = ((SolidColorBrush) _parent.Background).Color;

            NodeLinkGeomMakers[_parent.NodeLinkStyle].Make(
                _parent.LinksSource,
                lineCullingRect,
                alpha,
                backgroundColor,
                _parent.HighlightLinkColor,
                _curves);

            // 描画する
            dc.PushTransform(_parent.TranslateTransform);
            dc.PushTransform(_parent.ScaleTransform);
            {
                foreach (var curve in _curves)
                {
                    if (curve.Key.IsHightlight)
                        continue;

                    var pen =
                        (curve.Key.Style & BiaNodeLinkStyle.DashedLine) != 0
                            ? Caches.GetDashedPen(curve.Key.Color, LineWidth)
                            : Caches.GetPen(curve.Key.Color, LineWidth);

                    ((IDisposable) curve.Value.Ctx).Dispose();
                    dc.DrawGeometry(Caches.GetSolidColorBrush(curve.Key.Color), pen, curve.Value.Geom);
                }

                foreach (var curve in _curves)
                {
                    if (curve.Key.IsHightlight == false)
                        continue;

                    var pen =
                        (curve.Key.Style & BiaNodeLinkStyle.DashedLine) != 0
                            ? Caches.GetDashedPen(curve.Key.Color, LineWidth)
                            : Caches.GetPen(curve.Key.Color, LineWidth);

                    ((IDisposable) curve.Value.Ctx).Dispose();
                    dc.DrawGeometry(Caches.GetSolidColorBrush(curve.Key.Color), pen, curve.Value.Geom);
                }
            }
            dc.Pop();
            dc.Pop();

            _curves.Clear();
        }
    }
}