using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Collections;

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

        private readonly BiaNodeEditor _parent;
        private readonly ScaleTransform _scale;
        private readonly TranslateTransform _translate;

        static BackgroundPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
        }

        internal BackgroundPanel(BiaNodeEditor parent, ScaleTransform scale, TranslateTransform translate)
        {
            _parent = parent;
            _scale = scale;
            _translate = translate;

            _scale.Changed += (_, __) => InvalidateVisual();
            _translate.Changed += (_, __) => InvalidateVisual();
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

            var s = _scale.ScaleX;
            var tx = _translate.X;
            var ty = _translate.Y;

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

            dc.PushTransform(_translate);
            dc.PushTransform(_scale);
            {
                var viewport = _parent.MakeCurrentViewport();

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

        // http://proprogrammer.hatenadiary.jp/entry/2014/12/16/001014
        private static bool HitTestBezier(Point[] p, in ImmutableRect rect)
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

            return HitTestBezier(new[]
                   {
                       p[0],
                       v01,
                       v0112,
                       c
                   }, rect)
                   || HitTestBezier(new[]
                   {
                       c,
                       v1223,
                       v23,
                       p[3]
                   }, in rect);
        }
    }
}