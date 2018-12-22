using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly Point[] _bezierPoints = new Point[4];

        private readonly Dictionary<Color, (StreamGeometry Geom, StreamGeometryContext Ctx)> _curves =
            new Dictionary<Color, (StreamGeometry, StreamGeometryContext)>();

        private void DrawNodeLink(DrawingContext dc)
        {
            if (LinksSource == null)
                return;

            var viewport = _transform.MakeSceneRectFromControlPos(ActualWidth, ActualHeight);

            foreach (var link in LinksSource)
            {
                var (pos0, dir0) = link.Item0.MakePortPos(link.Item0PortId);
                var (pos1, dir1) = link.Item1.MakePortPos(link.Item1PortId);

                _bezierPoints[0] = pos0;
                _bezierPoints[1] = NodeEditorHelper.MakeBezierControlPoint(pos0, dir0);
                _bezierPoints[2] = NodeEditorHelper.MakeBezierControlPoint(pos1, dir1);
                _bezierPoints[3] = pos1;

                if (NodeEditorHelper.HitTestBezier(_bezierPoints, viewport) == false)
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

                curve.Ctx.BeginFigure(pos0, false, false);
                curve.Ctx.BezierTo(_bezierPoints[1], _bezierPoints[2], pos1, true, true);
            }

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                foreach (var c in _curves)
                {
                    var pen = Caches.GetBorderPen(c.Key, 8);

                    //c.Value.Ctx.Close();
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