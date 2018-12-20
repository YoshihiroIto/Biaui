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

                foreach (var link in LinksSource)
                {
                    var (pos0, dir0) = link.Item0.MakePortPos(link.Item0PortId);
                    var (pos1, dir1) = link.Item1.MakePortPos(link.Item1PortId);

                    var pos01 = MakeControlPoint(dir0, pos0);
                    var pos10 = MakeControlPoint(dir1, pos1);

                    if (HitTestBezier(new[]
                    {
                        (Vector) pos0,
                        (Vector) pos01,
                        (Vector) pos10,
                        (Vector) pos1
                    }, viewport) == false)
                        continue;

                    var pf = new PathFigure { StartPoint = pos0 };
                    var bs = new BezierSegment(pos01, pos10, pos1, true);

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
        private static bool HitTestBezier(Vector[] v, in ImmutableRect rect)
        {
            var p0 = new Point(v[0].X, v[0].Y);
            var p3 = new Point(v[3].X, v[3].Y);
            // 始点か終点を含んでいれば、Hitしている。
            if (rect.Contains(p0) || rect.Contains(p3))
                return true;

            var area = new Rect(p0, p3);
            area.Union(new ImmutableRect(new Point(v[1].X, v[1].Y), new Point(v[2].X, v[2].Y)));

            // ここらへんで小細工の余地はあります。幅や高さが1未満だったら直線と大差ないとか、いろいろ。

            // 交差している可能性があるケースを判定
            if (rect.IntersectsWith(area))
            {
                var v01 = (v[0] + v[1]) / 2;
                var v12 = (v[1] + v[2]) / 2;
                var v23 = (v[2] + v[3]) / 2;

                var v0112 = (v01 + v12) / 2;
                var v1223 = (v12 + v23) / 2;

                var c = (v0112 + v1223) / 2;

                // 2分割してさらに探査
                return HitTestBezier(new[]
                       {
                           v[0],
                           v01,
                           v0112,
                           c
                       }, rect)
                       || HitTestBezier(new[]
                       {
                           c,
                           v1223,
                           v23,
                           v[3]
                       }, in rect);
            }

            // 可能性が無い
            return false;
        }
    }
}