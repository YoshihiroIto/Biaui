using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeEditor : BiaClippingBorder, IHasTransform 
    {
        #region NodesSource

        public ObservableCollection<IBiaNodeItem> NodesSource
        {
            get => _NodesSource;
            set
            {
                if (value != _NodesSource)
                    SetValue(NodesSourceProperty, value);
            }
        }

        private ObservableCollection<IBiaNodeItem> _NodesSource;

        public static readonly DependencyProperty NodesSourceProperty =
            DependencyProperty.Register(nameof(NodesSource), typeof(ObservableCollection<IBiaNodeItem>),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(ObservableCollection<IBiaNodeItem>),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._NodesSource = (ObservableCollection<IBiaNodeItem>) e.NewValue;
                        self.InvokeNodesSourceChanging();
                    }));

        #endregion

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
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(ObservableCollection<IBiaNodeLink>),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;

                        var old = self._LinksSource;
                        self._LinksSource = (ObservableCollection<IBiaNodeLink>) e.NewValue;
                        self.UpdateLinksSource(old, self._LinksSource);
                    }));

        #endregion

        #region SourceNodePortConnecting

        public BiaNodeItemPortPair SourceNodePortConnecting
        {
            get => _sourceNodePortConnecting;
            set
            {
                if (value != _sourceNodePortConnecting)
                    SetValue(SourceNodePortConnectingProperty, value);
            }
        }

        private BiaNodeItemPortPair _sourceNodePortConnecting;

        public static readonly DependencyProperty SourceNodePortConnectingProperty =
            DependencyProperty.Register(
                nameof(SourceNodePortConnecting),
                typeof(BiaNodeItemPortPair),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(BiaNodeItemPortPair),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._sourceNodePortConnecting = (BiaNodeItemPortPair) e.NewValue;
                    }));

        #endregion

        #region TargetNodePortConnecting

        public BiaNodeItemPortPair TargetNodePortConnecting
        {
            get => _targetNodePortConnecting;
            set
            {
                if (value != _targetNodePortConnecting)
                    SetValue(TargetNodePortConnectingProperty, value);
            }
        }

        private BiaNodeItemPortPair _targetNodePortConnecting;

        public static readonly DependencyProperty TargetNodePortConnectingProperty =
            DependencyProperty.Register(
                nameof(TargetNodePortConnecting),
                typeof(BiaNodeItemPortPair),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(BiaNodeItemPortPair),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._targetNodePortConnecting = (BiaNodeItemPortPair) e.NewValue;
                    }));

        #endregion

        #region NodePortEnabledChecker

        public IBiaNodePortEnabledChecker NodePortEnabledChecker
        {
            get => _NodePortEnabledChecker;
            set
            {
                if (value != _NodePortEnabledChecker)
                    SetValue(NodePortEnabledCheckerProperty, value);
            }
        }

        private IBiaNodePortEnabledChecker _NodePortEnabledChecker;

        public static readonly DependencyProperty NodePortEnabledCheckerProperty =
            DependencyProperty.Register(
                nameof(NodePortEnabledChecker),
                typeof(IBiaNodePortEnabledChecker),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(IBiaNodePortEnabledChecker),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._NodePortEnabledChecker = (IBiaNodePortEnabledChecker) e.NewValue;
                    }));

        #endregion

        public event EventHandler<NodeLinkStartingEventArgs> NodeLinkStarting;

        public event EventHandler<NodeLinkCompletedEventArgs> NodeLinkCompleted;

        public ScaleTransform Scale { get; } = new ScaleTransform();

        public TranslateTransform Translate { get; } = new TranslateTransform();

        public bool IsNodePortDragging { get; internal set; }

        internal event EventHandler NodeItemMoved;

        internal event EventHandler NodesSourceChanging;

        internal event EventHandler LinksSourceChanging;

        internal event EventHandler LinkChanged;

        static BiaNodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(typeof(BiaNodeEditor)));
        }

        public BiaNodeEditor()
        {
            var mouseOperator = new MouseOperator(this, this);

            var grid = new Grid();
            grid.Children.Add(new NodeContainer(this, mouseOperator));
            grid.Children.Add(new BoxSelector(mouseOperator));
            grid.Children.Add(new NodePortConnector(this, mouseOperator));
            base.Child = grid;

            // 背景再描画
            Translate.Changed += (_, __) => InvalidateVisual();
            Scale.Changed += (_, __) => InvalidateVisual();
            NodeItemMoved += (_, __) => InvalidateVisual();
            LinksSourceChanging += (_, __) => InvalidateVisual();
            LinkChanged += (_, __) => InvalidateVisual();
            mouseOperator.PreMouseLeftButtonUp += (_, __) => InvalidateVisual();
        }

        private void UpdateLinksSource(
            ObservableCollection<IBiaNodeLink> oldSource,
            ObservableCollection<IBiaNodeLink> newSource)
        {
            LinksSourceChanging?.Invoke(this, EventArgs.Empty);

            if (oldSource != null)
                oldSource.CollectionChanged -= LinksSourceOnCollectionChanged;

            if (newSource != null)
                newSource.CollectionChanged += LinksSourceOnCollectionChanged;

            //////////////////////////////////////////////////////////////////////////////
            void LinksSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                => LinksSourceChanging?.Invoke(this, EventArgs.Empty);
        }

        internal void InvokeNodeLinkCompleted(NodeLinkCompletedEventArgs args)
            => NodeLinkCompleted?.Invoke(this, args);

        internal void InvokeNodeLinkStarting(NodeLinkStartingEventArgs args)
            => NodeLinkStarting?.Invoke(this, args);

        internal void InvokeNodeItemMoved()
            => NodeItemMoved?.Invoke(this, EventArgs.Empty);

        internal void InvokeNodesSourceChanging()
            => NodesSourceChanging?.Invoke(this, EventArgs.Empty);

        internal void InvokeLinkChanged()
            => LinkChanged?.Invoke(this, EventArgs.Empty);

        #region 背景

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            base.OnRender(dc);

            DrawGrid(dc);

            DrawNodeLink(dc);
        }

        private readonly StreamGeometry _gridGeom = new StreamGeometry();

        private void DrawGrid(DrawingContext dc)
        {
            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = Scale.ScaleX;
            var tx = Translate.X;
            var ty = Translate.Y;

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

            var viewport = this.TransformRect(ActualWidth, ActualHeight);

            var backgroundColor = ((SolidColorBrush)Background).Color;

            var alpha = IsNodePortDragging ? 0.2 : 1.0;

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

            dc.PushTransform(Translate);
            dc.PushTransform(Scale);
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

        #endregion
    }
}