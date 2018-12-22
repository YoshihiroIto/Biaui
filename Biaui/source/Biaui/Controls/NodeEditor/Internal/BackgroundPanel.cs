using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            for (var h = 0;; ++h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x < 0) continue;

                if (x > ActualWidth) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var h = 0;; --h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x > ActualWidth) continue;

                if (x < 0) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var v = 0;; ++v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y < 0) continue;

                if (y > ActualHeight) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }

            for (var v = 0;; --v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y > ActualHeight) continue;

                if (y < 0) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }
        }

        private void DrawNodeLink(DrawingContext dc)
        {
            if (LinksSource == null)
                return;

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                var viewport = _transform.MakeSceneRectFromControlPos(ActualWidth, ActualHeight);

                var penO = Caches.GetBorderPen(Colors.Black, FrameworkElementHelper.RoundLayoutValue(8));
                var penI = Caches.GetBorderPen(Colors.LimeGreen, FrameworkElementHelper.RoundLayoutValue(6));

                var pv = new Point[4];

                foreach (var link in LinksSource)
                {
                    var (pos0, dir0) = link.Item0.MakePortPos(link.Item0PortId);
                    var (pos1, dir1) = link.Item1.MakePortPos(link.Item1PortId);

                    pv[0] = pos0;
                    pv[1] = MakeControlPoint(dir0, pos0);
                    pv[2] = MakeControlPoint(dir1, pos1);
                    pv[3] = pos1;

                    if (HitTestBezier(pv, viewport) == false)
                        continue;

                    var pf = new PathFigure { StartPoint = pos0 };
                    var bs = new BezierSegment(pv[1], pv[2], pos1, true);

                    pf.Segments.Add(bs);

                    var curve = new PathGeometry();
                    curve.Figures.Add(pf);

                    dc.DrawGeometry(null, penO, curve);
                    dc.DrawGeometry(null, penI, curve);
                }
            }
            dc.Pop();
            dc.Pop();
        }

        private const double ControlPointLength = 128;

        private static Point MakeControlPoint(BiaNodePortDir dir, Point src)
        {
            switch (dir)
            {
                case BiaNodePortDir.Left:
                    return new Point(src.X - ControlPointLength, src.Y);

                case BiaNodePortDir.Top:
                    return new Point(src.X, src.Y - ControlPointLength);

                case BiaNodePortDir.Right:
                    return new Point(src.X + ControlPointLength, src.Y);

                case BiaNodePortDir.Bottom:
                    return new Point(src.X, src.Y + ControlPointLength);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool HitTestBezier(Point[] p, in ImmutableRect rect)
        {
            while (true)
            {
                if (rect.Contains(p[0]) || rect.Contains(p[3]))
                    return true;

                var area = new ImmutableRect(p);

                if (rect.IntersectsWith(area) == false)
                    return false;

                var v01 = new Point((p[0].X + p[1].X) * 0.5, (p[0].Y + p[1].Y) * 0.5);
                var v12 = new Point((p[1].X + p[2].X) * 0.5, (p[1].Y + p[2].Y) * 0.5);
                var v23 = new Point((p[2].X + p[3].X) * 0.5, (p[2].Y + p[3].Y) * 0.5);

                var v0112 = new Point((v01.X + v12.X) * 0.5, (v01.Y + v12.Y) * 0.5);
                var v1223 = new Point((v12.X + v23.X) * 0.5, (v12.Y + v23.Y) * 0.5);

                var c = new Point((v0112.X + v1223.X) * 0.5, (v0112.Y + v1223.Y) * 0.5);

                var cl = new[]
                {
                    p[0],
                    v01,
                    v0112,
                    c
                };

                if (HitTestBezier(cl, rect))
                    return true;

                cl[0] = c;
                cl[1] = v1223;
                cl[2] = v23;
                cl[3] = p[3];

                p = cl;
            }
        }
    }
}