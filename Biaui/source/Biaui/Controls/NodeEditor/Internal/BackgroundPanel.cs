using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BackgroundPanel : Canvas
    {
        #region LinksSource

        public ObservableCollection<IBiaNodeLink> LinksSource
        {
            get => _LinksSource;
            set
            {
                if (value != _LinksSource)
                    SetValue(LinksSourceProperty, value);
            }
        }

        private ObservableCollection<IBiaNodeLink> _LinksSource;

        public static readonly DependencyProperty LinksSourceProperty =
            DependencyProperty.Register(nameof(LinksSource), typeof(ObservableCollection<IBiaNodeLink>),
                typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(
                    default(ObservableCollection<IBiaNodeLink>),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BackgroundPanel) s;
                        self._LinksSource = (ObservableCollection<IBiaNodeLink>) e.NewValue;
                    }));

        #endregion

        private readonly IHasTransform _transform;
        private readonly IHasIsNodePortDragging _hasIsNodePortDragging;

        static BackgroundPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
        }

        internal BackgroundPanel(IHasTransform transform, IHasIsNodePortDragging hasIsNodePortDragging, MouseOperator mouseOperator)
        {
            _transform = transform;
            _hasIsNodePortDragging = hasIsNodePortDragging;

            _transform.Translate.Changed += (_, __) => InvalidateVisual();
            _transform.Scale.Changed += (_, __) => InvalidateVisual();

            mouseOperator.MouseLeftButtonUp += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawGrid(dc);

            DrawNodeLink(dc);
        }

        private readonly StreamGeometry _gridGeom = new StreamGeometry();

        private void DrawGrid(DrawingContext dc)
        {
            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _transform.Scale.ScaleX;
            var tx = _transform.Translate.X;
            var ty = _transform.Translate.Y;

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

        private static readonly
            Dictionary<(Color Color, BiaNodeLinkStyle Style), (StreamGeometry Geom, StreamGeometryContext Ctx)> _curves
                = new Dictionary<(Color, BiaNodeLinkStyle), (StreamGeometry, StreamGeometryContext)>();

        private void DrawNodeLink(DrawingContext dc)
        {
            if (LinksSource == null)
                return;

            Span<Point> bezierPoints = stackalloc Point[4];
            var hitTestWork = MemoryMarshal.Cast<Point, ImmutableVec2>(bezierPoints);

            var viewport = _transform.TransformRect(ActualWidth, ActualHeight);

            var colorRes = TryFindResource("TextBoxBackgroundColorKey");
            var backgroundColor = (Color?) colorRes ?? Colors.Black;

            var alpha = _hasIsNodePortDragging.IsNodePortDragging ? 0.2 : 1.0;

            foreach (var link in LinksSource)
            {
                InternalBiaNodeLinkData internalData;

                if (link.InternalData == null)
                {
                    internalData = new InternalBiaNodeLinkData
                    {
                        Port1 = link.ItemPort1.FindPort(),
                        Port2 = link.ItemPort2.FindPort()
                    };

                    link.InternalData = internalData;
                }
                else
                {
                    internalData = (InternalBiaNodeLinkData) link.InternalData;
                }

                if (internalData.Port1 == null || internalData.Port2 == null)
                    continue;

                var pos1 = link.ItemPort1.Item.MakePortPos(internalData.Port1);
                var pos2 = link.ItemPort2.Item.MakePortPos(internalData.Port2);

                var pos12 = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, internalData.Port1.Dir);
                var pos21 = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, internalData.Port2.Dir);

                // ※.HitTestBezier を呼ぶと_bezierPointsは書き変わる
                bezierPoints[0] = pos1;
                bezierPoints[1] = pos12;
                bezierPoints[2] = pos21;
                bezierPoints[3] = pos2;

                if (BiaNodeEditorHelper.HitTestBezier(hitTestWork, viewport) == false)
                    continue;

                var color = ColorHelper.Lerp(alpha, backgroundColor, link.Color);
                var key = (color, link.Style);

                // 線
                if (_curves.TryGetValue(key, out var curve) == false)
                {
                    var geom = new StreamGeometry
                    {
                        FillRule = FillRule.Nonzero
                    };
                    var ctx = geom.Open();

                    curve = (geom, ctx);
                    _curves.Add(key, curve);
                }

                curve.Ctx.BeginFigure(pos1, false, false);
                curve.Ctx.BezierTo(pos12, pos21, pos2, true, true);

                // 矢印
                if ((link.Style & BiaNodeLinkStyle.Arrow) != 0)
                {
                    var p1 = BiaNodeEditorHelper.InterpolationBezier(pos1, pos12, pos21, pos2, 0.50);
                    var p2 = BiaNodeEditorHelper.InterpolationBezier(pos1, pos12, pos21, pos2, 0.45);

                    const double size = 20;
                    var pv = ImmutableVec2.SetSize(p1 - p2, size);
                    var sv = new ImmutableVec2(-pv.Y / 1.732, pv.X / 1.732);

                    var t1 = p1 + pv;
                    var t2 = p1 + sv;
                    var t3 = p1 - sv;

                    curve.Ctx.DrawTriangle(
                        Unsafe.As<ImmutableVec2, Point>(ref t1),
                        Unsafe.As<ImmutableVec2, Point>(ref t2),
                        Unsafe.As<ImmutableVec2, Point>(ref t3),
                        false, false);
                }
            }

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                foreach (var c in _curves)
                {
                    var pen =
                        (c.Key.Style & BiaNodeLinkStyle.DashedLine) != 0
                            ? Caches.GetDashedPen(c.Key.Color, 3)
                            : Caches.GetPen(c.Key.Color, 3);

                    ((IDisposable) c.Value.Ctx).Dispose();
                    dc.DrawGeometry(Caches.GetSolidColorBrush(c.Key.Color), pen, c.Value.Geom);
                }
            }
            dc.Pop();
            dc.Pop();

            _curves.Clear();
        }
    }
}