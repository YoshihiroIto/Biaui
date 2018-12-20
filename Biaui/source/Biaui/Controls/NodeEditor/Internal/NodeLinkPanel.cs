using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class NodeLinkPanel : Canvas
    {
        #region NodesSource

        public ObservableCollection<INodeItem> NodesSource
        {
            get => _NodesSource;
            set
            {
                if (value != _NodesSource)
                    SetValue(NodesSourceProperty, value);
            }
        }

        private ObservableCollection<INodeItem> _NodesSource;

        public static readonly DependencyProperty NodesSourceProperty =
            DependencyProperty.Register(nameof(NodesSource), typeof(ObservableCollection<INodeItem>),
                typeof(NodeLinkPanel),
                new FrameworkPropertyMetadata(
                    default(ObservableCollection<INodeItem>),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (NodeLinkPanel) s;
                        self._NodesSource = (ObservableCollection<INodeItem>) e.NewValue;
                    }));

        #endregion

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
                typeof(NodeLinkPanel),
                new FrameworkPropertyMetadata(
                    default(ObservableCollection<ILinkItem>),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (NodeLinkPanel) s;
                        self._LinksSource = (ObservableCollection<ILinkItem>) e.NewValue;
                    }));

        #endregion

        private readonly BiaNodeEditor _parent;

        static NodeLinkPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeLinkPanel),
                new FrameworkPropertyMetadata(typeof(NodeLinkPanel)));
        }

        internal NodeLinkPanel(BiaNodeEditor parent, ScaleTransform scale, TranslateTransform translate)
        {
            _parent = parent;

            var g = new TransformGroup();
            g.Children.Add(scale);
            g.Children.Add(translate);
            RenderTransform = g;

            g.Changed += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

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
    }
}