using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BackgroundPanel : Canvas
    {
        #region LinksSource

        public ObservableCollection<ILinkItem> LinksSource
        {
            get => _LinksSource;
            set
            {
                if (value != _LinksSource)
                    SetValue(LinksSourceProperty, value);
            }
        }

        private ObservableCollection<ILinkItem> _LinksSource;

        public static readonly DependencyProperty LinksSourceProperty =
            DependencyProperty.Register(nameof(LinksSource), typeof(ObservableCollection<ILinkItem>),
                typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(
                    default(ObservableCollection<ILinkItem>),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BackgroundPanel) s;
                        self._LinksSource = (ObservableCollection<ILinkItem>) e.NewValue;
                    }));

        #endregion

        private readonly IHasTransform _transform;

        static BackgroundPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
        }

        internal BackgroundPanel(IHasTransform transform)
        {
            IsHitTestVisible = false;

            _transform = transform;

            _transform.Translate.Changed += (_, __) => InvalidateVisual();
            _transform.Scale.Changed += (_, __) => InvalidateVisual();
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

            var sgc = _gridGeom.Open();
            {
                for (var h = 0;; ++h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x < 0) continue;

                    if (x > ActualWidth) break;

                    sgc.BeginFigure(new Point(x, 0), false, false);
                    sgc.LineTo(new Point(x, by), true, false);
                }

                for (var h = 0;; --h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x > ActualWidth) continue;

                    if (x < 0) break;

                    sgc.BeginFigure(new Point(x, 0), false, false);
                    sgc.LineTo(new Point(x, by), true, false);
                }

                for (var v = 0;; ++v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y < 0) continue;

                    if (y > ActualHeight) break;

                    sgc.BeginFigure(new Point(0, y), false, false);
                    sgc.LineTo(new Point(bx, y), true, false);
                }

                for (var v = 0;; --v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y > ActualHeight) continue;

                    if (y < 0) break;

                    sgc.BeginFigure(new Point(0, y), false, false);
                    sgc.LineTo(new Point(bx, y), true, false);
                }
            }
            sgc.Close();
            dc.DrawGeometry(null, p, _gridGeom);
        }

        private readonly Dictionary<Color, (StreamGeometry Geom, StreamGeometryContext Ctx)> _curves =
            new Dictionary<Color, (StreamGeometry, StreamGeometryContext)>();

        private void DrawNodeLink(DrawingContext dc)
        {
            if (LinksSource == null)
                return;

            Span<Point> bezierPoints = stackalloc Point[4];
            var hitTestWork = MemoryMarshal.Cast<Point, ImmutableVec2>(bezierPoints);

            var viewport = _transform.MakeSceneRectFromControlPos(ActualWidth, ActualHeight);

            foreach (var link in LinksSource)
            {
                (BiaNodePort Port1, BiaNodePort Port2) portPair;

                if (link.InternalData == null)
                {
                    var port1 = link.Item1.Layout.FindPort(link.Item1PortId);
                    var port2 = link.Item2.Layout.FindPort(link.Item2PortId);

                    portPair = (port1, port2);

                    link.InternalData = portPair;
                }
                else
                {
                    portPair = ((BiaNodePort, BiaNodePort))link.InternalData;
                }

                var (pos1, dir1) = link.Item1.MakePortPos(portPair.Port1);
                var (pos2, dir2) = link.Item2.MakePortPos(portPair.Port2);

                var pos12 = NodeEditorHelper.MakeBezierControlPoint(pos1, dir1);
                var pos21 = NodeEditorHelper.MakeBezierControlPoint(pos2, dir2);

                // ※.HitTestBezier を呼ぶと_bezierPointsは書き変わる
                bezierPoints[0] = pos1;
                bezierPoints[1] = pos12;
                bezierPoints[2] = pos21;
                bezierPoints[3] = pos2;

                if (NodeEditorHelper.HitTestBezier(hitTestWork, viewport) == false)
                    continue;

                // todo;接続ごとの色を指定する
                var color = Colors.DeepPink;

                if (_curves.TryGetValue(color, out var curve) == false)
                {
                    var geom = new StreamGeometry();
                    var ctx = geom.Open();

                    curve = (geom, ctx);
                    _curves.Add(color, curve);
                }

                curve.Ctx.BeginFigure(pos1, false, false);
                curve.Ctx.BezierTo(pos12, pos21, pos2, true, true);
            }

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                foreach (var c in _curves)
                {
                    var pen = Caches.GetBorderPen(c.Key, 8);

                    (c.Value.Ctx as IDisposable).Dispose();
                    dc.DrawGeometry(null, pen, c.Value.Geom);
                }
            }
            dc.Pop();
            dc.Pop();

            _curves.Clear();
        }
    }
}