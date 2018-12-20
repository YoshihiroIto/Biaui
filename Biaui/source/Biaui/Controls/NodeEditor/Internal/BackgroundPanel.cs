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

                var pen = Caches.GetBorderPen(Colors.LimeGreen, FrameworkElementHelper.RoundLayoutValue(8));
                var penO = Caches.GetBorderPen(Colors.Black, FrameworkElementHelper.RoundLayoutValue(10));

                foreach (var link in LinksSource)
                {
                    var pos0 = link.Item0.MakePortPos(link.Item0PortId);
                    var pos1 = link.Item1.MakePortPos(link.Item1PortId);

                    var rect = new ImmutableRect(pos0, pos1);
                    if (viewport.IntersectsWith(rect) == false)
                        continue;

                    dc.DrawLine(penO, pos0, pos1);
                    dc.DrawLine(pen, pos0, pos1);
                }
            }
            dc.Pop();
            dc.Pop();
        }
    }
}