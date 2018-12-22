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

        private void DrawGrid(DrawingContext dc)
        {
            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _transform.Scale.ScaleX;
            var tx = _transform.Translate.X;
            var ty = _transform.Translate.Y;

            var bx = FrameworkElementHelper.RoundLayoutValue(ActualWidth);
            var by = FrameworkElementHelper.RoundLayoutValue(ActualHeight);

            var geom = new StreamGeometry();
            var sgc = geom.Open();
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
            dc.DrawGeometry(null, p, geom);
        }

        private readonly Point[] _bezierPoints = new Point[4];

        private void DrawNodeLink(DrawingContext dc)
        {
            if (LinksSource == null)
                return;

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                var pen = Caches.GetBorderPen(Colors.DeepPink, FrameworkElementHelper.RoundLayoutValue(6));
                var viewport = _transform.MakeSceneRectFromControlPos(ActualWidth, ActualHeight);

#if false
                // 接続ごとに色を変える場合は一本ずつ書く
                foreach (var link in LinksSource)
                {
                    var (pos0, dir0) = link.Item0.MakePortPos(link.Item0PortId);
                    var (pos1, dir1) = link.Item1.MakePortPos(link.Item1PortId);

                    _bezierPoints[0] = pos0;
                    _bezierPoints[1] = MakeControlPoint(dir0, pos0);
                    _bezierPoints[2] = MakeControlPoint(dir1, pos1);
                    _bezierPoints[3] = pos1;

                    if (HitTestBezier(_bezierPoints, viewport) == false)
                        continue;

                    var pf = new PathFigure
                    {
                        StartPoint = pos0
                    };
                    var bs = new BezierSegment(_bezierPoints[1], _bezierPoints[2], pos1, true);

                    pf.Segments.Add(bs);

                    var curve = new PathGeometry();
                    curve.Figures.Add(pf);

                    dc.DrawGeometry(null, pen, curve);
                }
#else
                // すべて同じ色の場合はまとめて書く
                {
                    var geom = new StreamGeometry();
                    var sgc = geom.Open();

                    foreach (var link in LinksSource)
                    {
                        var (pos0, dir0) = link.Item0.MakePortPos(link.Item0PortId);
                        var (pos1, dir1) = link.Item1.MakePortPos(link.Item1PortId);

                        _bezierPoints[0] = pos0;
                        _bezierPoints[1] = NodeEditorHelper.MakeControlPoint(pos0, dir0);
                        _bezierPoints[2] = NodeEditorHelper.MakeControlPoint(pos1, dir1);
                        _bezierPoints[3] = pos1;

                        if (NodeEditorHelper.HitTestBezier(_bezierPoints, viewport) == false)
                            continue;

                        sgc.BeginFigure(pos0, false, false);
                        sgc.BezierTo(_bezierPoints[1], _bezierPoints[2], pos1, true, true);
                    }

                    sgc.Close();
                    dc.DrawGeometry(null, pen, geom);
                }
#endif
            }
            dc.Pop();
            dc.Pop();
        }
    }
}