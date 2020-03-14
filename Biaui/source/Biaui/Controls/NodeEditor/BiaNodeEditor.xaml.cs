using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Environment;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    public enum BiaNodeEditorNodeLinkStyle
    {
        AxisAlign,
        BezierCurve
    }

    public class BiaNodeEditor : BiaClippingBorder, IHasTransform, IHasScalerRange
    {
        #region NodesSource

        public IEnumerable? NodesSource
        {
            get => _NodesSource;
            set
            {
                if (!Equals(value, _NodesSource))
                    SetValue(NodesSourceProperty, value);
            }
        }

        private IEnumerable? _NodesSource;

        public static readonly DependencyProperty NodesSourceProperty =
            DependencyProperty.Register(nameof(NodesSource), typeof(IEnumerable),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(IEnumerable),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._NodesSource = (IEnumerable) e.NewValue;
                        self.InvokeNodesSourceChanging();
                    }));

        #endregion

        #region LinksSource

        public IEnumerable? LinksSource
        {
            get => _LinksSource;
            set
            {
                if (!Equals(value, _LinksSource))
                    SetValue(LinksSourceProperty, value);
            }
        }

        private IEnumerable? _LinksSource;

        public static readonly DependencyProperty LinksSourceProperty =
            DependencyProperty.Register(nameof(LinksSource), typeof(IEnumerable),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(IEnumerable),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;

                        var old = self._LinksSource;
                        self._LinksSource = (IEnumerable) e.NewValue;
                        self.UpdateLinksSource(old, self._LinksSource);
                    }));

        #endregion

        #region SourceNodeSlotConnecting

        public BiaNodeItemSlotPair SourceNodeSlotConnecting
        {
            get => _sourceNodeSlotConnecting;
            set
            {
                if (value != _sourceNodeSlotConnecting)
                    SetValue(SourceNodeSlotConnectingProperty, value);
            }
        }

        private BiaNodeItemSlotPair _sourceNodeSlotConnecting;

        public static readonly DependencyProperty SourceNodeSlotConnectingProperty =
            DependencyProperty.Register(
                nameof(SourceNodeSlotConnecting),
                typeof(BiaNodeItemSlotPair),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(BiaNodeItemSlotPair),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._sourceNodeSlotConnecting = (BiaNodeItemSlotPair) e.NewValue;
                    }));

        #endregion

        #region TargetNodeSlotConnecting

        public BiaNodeItemSlotPair TargetNodeSlotConnecting
        {
            get => _targetNodeSlotConnecting;
            set
            {
                if (value != _targetNodeSlotConnecting)
                    SetValue(TargetNodeSlotConnectingProperty, value);
            }
        }

        private BiaNodeItemSlotPair _targetNodeSlotConnecting;

        public static readonly DependencyProperty TargetNodeSlotConnectingProperty =
            DependencyProperty.Register(
                nameof(TargetNodeSlotConnecting),
                typeof(BiaNodeItemSlotPair),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(BiaNodeItemSlotPair),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._targetNodeSlotConnecting = (BiaNodeItemSlotPair) e.NewValue;
                    }));

        #endregion

        #region NodeSlotEnabledChecker

        public IBiaNodeSlotEnabledChecker? NodeSlotEnabledChecker
        {
            get => _nodeSlotEnabledChecker;
            set
            {
                if (value != _nodeSlotEnabledChecker)
                    SetValue(NodeSlotEnabledCheckerProperty, value);
            }
        }

        private IBiaNodeSlotEnabledChecker? _nodeSlotEnabledChecker;

        public static readonly DependencyProperty NodeSlotEnabledCheckerProperty =
            DependencyProperty.Register(
                nameof(NodeSlotEnabledChecker),
                typeof(IBiaNodeSlotEnabledChecker),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(IBiaNodeSlotEnabledChecker),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._nodeSlotEnabledChecker = (IBiaNodeSlotEnabledChecker) e.NewValue;
                    }));

        #endregion

        #region HighlightLinkColor

        public ByteColor HighlightLinkColor
        {
            get => _HighlightLinkColor;
            set
            {
                if (value != _HighlightLinkColor)
                    SetValue(HighlightLinkColorProperty, value);
            }
        }

        private ByteColor _HighlightLinkColor = ByteColor.GhostWhite;

        public static readonly DependencyProperty HighlightLinkColorProperty =
            DependencyProperty.Register(
                nameof(HighlightLinkColor),
                typeof(ByteColor),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    ByteColor.GhostWhite,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._HighlightLinkColor = (ByteColor) e.NewValue;
                    }));

        #endregion

        #region NodeLinkStyle

        public BiaNodeEditorNodeLinkStyle NodeLinkStyle
        {
            get => _NodeLinkStyle;
            set
            {
                if (value != _NodeLinkStyle)
                    SetValue(NodeLinkStyleProperty, Boxes.NodeEditorNodeLinkStyle(value));
            }
        }

        private BiaNodeEditorNodeLinkStyle _NodeLinkStyle = BiaNodeEditorNodeLinkStyle.AxisAlign;

        public static readonly DependencyProperty NodeLinkStyleProperty =
            DependencyProperty.Register(nameof(NodeLinkStyle), typeof(BiaNodeEditorNodeLinkStyle),
                typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(
                    Boxes.NodeEditorNodeLinkStyleAxisAlign,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._NodeLinkStyle = (BiaNodeEditorNodeLinkStyle) e.NewValue;
                    }));

        #endregion

        #region Scale

        public double Scale
        {
            get => _Scale;
            set
            {
                if (NumberHelper.AreClose(value, Scale) == false)
                    SetValue(ScaleProperty, Boxes.Double(value));
            }
        }

        private double _Scale = 1.0;

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                nameof(Scale),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._Scale = (double) e.NewValue;

                        if (self._isInTransformChanging == false)
                        {
                            self._isInTransformChanging = true;
                            self.ScaleTransform.ScaleX = self._Scale;
                            self.ScaleTransform.ScaleY = self._Scale;
                            self._isInTransformChanging = false;
                        }
                    }));

        #endregion

        #region TranslateX

        public double TranslateX
        {
            get => _TranslateX;
            set
            {
                if (NumberHelper.AreClose(value, _TranslateX) == false)
                    SetValue(TranslateXProperty, Boxes.Double(value));
            }
        }

        private double _TranslateX;

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register(
                nameof(TranslateX),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._TranslateX = (double) e.NewValue;

                        if (self._isInTransformChanging == false)
                        {
                            self._isInTransformChanging = true;
                            self.TranslateTransform.X = self._TranslateX;
                            self._isInTransformChanging = false;
                        }
                    }));

        #endregion

        #region TranslateY

        public double TranslateY
        {
            get => _TranslateY;
            set
            {
                if (NumberHelper.AreClose(value, _TranslateY) == false)
                    SetValue(TranslateYProperty, Boxes.Double(value));
            }
        }

        private double _TranslateY;

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register(
                nameof(TranslateY),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._TranslateY = (double) e.NewValue;

                        if (self._isInTransformChanging == false)
                        {
                            self._isInTransformChanging = true;
                            self.TranslateTransform.Y = self._TranslateY;
                            self._isInTransformChanging = false;
                        }
                    }));

        #endregion

        #region CanConnectLink

        public bool CanConnectLink
        {
            get => _CanConnectLink;
            set
            {
                if (value != _CanConnectLink)
                    SetValue(CanConnectLinkProperty, Boxes.Bool(value));
            }
        }

        private bool _CanConnectLink = true;

        public static readonly DependencyProperty CanConnectLinkProperty =
            DependencyProperty.Register(
                nameof(CanConnectLink),
                typeof(bool),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._CanConnectLink = (bool) e.NewValue;
                    }));

        #endregion

        #region OverlayHeaderHeight

        public double OverlayHeaderHeight
        {
            get => _OverlayHeaderHeight;
            set
            {
                if (NumberHelper.AreClose(value, _OverlayHeaderHeight) == false)
                    SetValue(OverlayHeaderHeightProperty, Boxes.Double(value));
            }
        }

        private double _OverlayHeaderHeight;

        public static readonly DependencyProperty OverlayHeaderHeightProperty =
            DependencyProperty.Register(
                nameof(OverlayHeaderHeight),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._OverlayHeaderHeight = (double) e.NewValue;
                    }));

        #endregion

        #region ScalerMaximum

        public double ScalerMaximum
        {
            get => _ScalerMaximum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _ScalerMaximum)
                    SetValue(ScalerMaximumProperty, value);
            }
        }

        private double _ScalerMaximum = 2d;

        public static readonly DependencyProperty ScalerMaximumProperty =
            DependencyProperty.Register(
                nameof(ScalerMaximum),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double2,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._ScalerMaximum = (double) e.NewValue;
                    }));

        #endregion

        #region ScalerMinimum

        public double ScalerMinimum
        {
            get => _ScalerMinimum;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _ScalerMinimum)
                    SetValue(ScalerMinimumProperty, value);
            }
        }

        private double _ScalerMinimum = 0.25;

        public static readonly DependencyProperty ScalerMinimumProperty =
            DependencyProperty.Register(
                nameof(ScalerMinimum),
                typeof(double),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    0.25,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._ScalerMinimum = (double) e.NewValue;
                    }));

        #endregion

        public ScaleTransform ScaleTransform { get; } = new ScaleTransform();

        public TranslateTransform TranslateTransform { get; } = new TranslateTransform();

        public bool IsNodeSlotDragging { get; internal set; }

        public event EventHandler<NodeLinkStartingEventArgs>? NodeLinkStarting;

        public event EventHandler<NodeLinkCompletedEventArgs>? NodeLinkCompleted;

        public event EventHandler? PropertyEditStarting;

        public event EventHandler? PropertyEditCompleted;

        public event EventHandler? ScaleTransformChanged;

        public event EventHandler? TranslateTransformChanged;

        internal event EventHandler? NodeItemMoved;

        internal event EventHandler? NodesSourceChanging;

        internal event EventHandler? LinksSourceChanging;

        internal event EventHandler? LinkChanged;

        private bool _isInTransformChanging;

        static BiaNodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(typeof(BiaNodeEditor)));
        }

        internal readonly NodeContainer[] NodeContainers = new NodeContainer[3];

        private readonly MouseOperator _mouseOperator;

        public BiaNodeEditor()
        {
            _mouseOperator = new MouseOperator(this, this, this);

            for (var i = 0; i != NodeContainers.Length; ++i)
                NodeContainers[i] = new NodeContainer(this, (BiaNodePanelLayer) i, _mouseOperator);

            SetupPropertyEditCommand();

            IBackgroundPanel backgroundPanel;
            var grid = new Grid();
            {
                backgroundPanel = BiauiEnvironment.BackgroundPanelGenerator.Generate(this);

                grid.Children.Add((UIElement) backgroundPanel);

                foreach (var nodeContainer in NodeContainers)
                    grid.Children.Add(nodeContainer);

                grid.Children.Add(new BoxSelector(_mouseOperator));
                grid.Children.Add(new NodeSlotConnector(this, _mouseOperator));
                grid.Children.Add(CreateScaleSlider());
            }

            base.Child = grid;

            ScaleTransform.Changed += (_, __) =>
            {
                if (_isInTransformChanging == false)
                {
                    _isInTransformChanging = true;
                    Scale = ScaleTransform.ScaleX;
                    _isInTransformChanging = false;
                }

                ScaleTransformChanged?.Invoke(this, EventArgs.Empty);
            };
            TranslateTransform.Changed += (_, __) =>
            {
                if (_isInTransformChanging == false)
                {
                    _isInTransformChanging = true;
                    TranslateX = TranslateTransform.X;
                    TranslateY = TranslateTransform.Y;
                    _isInTransformChanging = false;
                }

                TranslateTransformChanged?.Invoke(this, EventArgs.Empty);
            };

            // backgroundPanel
            {
                TranslateTransform.Changed += (_, __) => backgroundPanel.Invalidate();
                ScaleTransform.Changed += (_, __) => backgroundPanel.Invalidate();
                NodeItemMoved += (_, __) => backgroundPanel.Invalidate();
                LinksSourceChanging += (_, __) => backgroundPanel.Invalidate();
                LinkChanged += (_, __) => backgroundPanel.Invalidate();
                _mouseOperator.PreMouseLeftButtonUp += (_, __) => backgroundPanel.Invalidate();
            }

            _mouseOperator.OperationChanged += (_, __) =>
            {
                Cursor = _mouseOperator.IsEditorScroll
                    ? Cursors.SizeAll
                    : Cursors.Arrow;
            };
        }

        private void UpdateLinksSource(
            IEnumerable? oldSource,
            IEnumerable? newSource)
        {
            LinksSourceChanging?.Invoke(this, EventArgs.Empty);

            if (oldSource is INotifyCollectionChanged observableOldSource)
                observableOldSource.CollectionChanged -= LinksSourceOnCollectionChanged;

            if (newSource is INotifyCollectionChanged observableNewSource)
                observableNewSource.CollectionChanged += LinksSourceOnCollectionChanged;

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

        internal void InvokePropertyEditStarting()
            => PropertyEditStarting?.Invoke(this, EventArgs.Empty);

        internal void InvokePropertyEditCompleted()
            => PropertyEditCompleted?.Invoke(this, EventArgs.Empty);

        private Slider CreateScaleSlider()
        {
            var scaleSlider = new Slider
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(8),
                Height = 200,
                Value = ScaleTransform.ScaleX
            };

            BindingOperations.SetBinding(scaleSlider, RangeBase.MaximumProperty,
                new Binding
                {
                    Path = new PropertyPath(nameof(ScalerMaximum)),
                    Mode = BindingMode.OneWay,
                    Source = this
                }
            );

            BindingOperations.SetBinding(scaleSlider, RangeBase.MinimumProperty,
                new Binding
                {
                    Path = new PropertyPath(nameof(ScalerMinimum)),
                    Mode = BindingMode.OneWay,
                    Source = this
                }
            );

            scaleSlider.ValueChanged += (_, __) =>
            {
                var cx = ActualWidth * 0.5;
                var cy = ActualHeight * 0.5;

                this.SetTransform(scaleSlider.Value, cx, cy);
            };

            ScaleTransform.Changed += (_, __) => scaleSlider.Value = ScaleTransform.ScaleX;

            return scaleSlider;
        }

        private bool _isInEditing;

        private void SetupPropertyEditCommand()
        {
            _mouseOperator.PostMouseLeftButtonDown += (_, __) =>
            {
                // キー操作を受け付けるためにフォーカスをあてる
                Focus();

                _isInEditing = _mouseOperator.IsBoxSelect ||
                               _mouseOperator.IsPanelMove;

                if (_isInEditing == false)
                    return;

                InvokePropertyEditStarting();
            };

            _mouseOperator.PostMouseLeftButtonUp += (_, __) =>
            {
                if (_isInEditing == false)
                    return;

                InvokePropertyEditCompleted();
                _isInEditing = false;
            };
        }

        public void Sleep()
        {
            foreach (var nodeContainer in NodeContainers)
                ++nodeContainer.IsEnableUpdateChildrenBagDepth;
        }

        public void Wakeup()
        {
            foreach (var nodeContainer in NodeContainers)
            {
                --nodeContainer.IsEnableUpdateChildrenBagDepth;
                Debug.Assert(nodeContainer.IsEnableUpdateChildrenBagDepth >= 0);

                if (nodeContainer.IsEnableUpdateChildrenBagDepth == 0)
                    nodeContainer.UpdateChildrenBag();
            }
        }

        #region Fit

        public void FitAllNodes()
        {
            if (NodesSource == null)
                return;

            FitNodesInternal(NodesSource.Cast<IBiaNodeItem>());
        }

        public void FitSelectedNodes()
        {
            if (NodesSource == null)
                return;

            FitNodesInternal(NodesSource.Cast<IBiaNodeItem>().Where(x => x.IsSelected));
        }

        private void FitNodesInternal(IEnumerable<IBiaNodeItem> targetNodes)
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var node in targetNodes)
            {
                if (node.Flags.HasFlag(BiaNodePaneFlags.DesktopSpace))
                    continue;
                
                minX = (minX, node.Pos.X).Min();
                maxX = (maxX, node.Pos.X + node.Size.Width).Max();
                minY = (minY, node.Pos.Y).Min();
                maxY = (maxY, node.Pos.Y + node.Size.Height).Max();
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (minX == double.MaxValue)
                return;

            const double margin = 128.0;
            minX -= margin;
            minY -= margin;
            maxX += margin;
            maxY += margin;

            // ---------------------------------------------------------------
            var centerX = (minX + maxX) * 0.5;
            var centerY = (minY + maxY) * 0.5;
            var w = maxX - minX;
            var h = maxY - minY;

            var width = ActualWidth;
            var height = ActualHeight - OverlayHeaderHeight;

            var scaleX = width / w;
            var scaleY = height / h;

            var viewCx = width * 0.5;
            var viewCy = height * 0.5;

            var scale = (scaleX, scaleY).Min();
            scale = (scale, ScalerMinimum, ScalerMaximum).Clamp();

            TranslateTransform.X = -centerX * scale + viewCx;
            TranslateTransform.Y = -centerY * scale + viewCy + OverlayHeaderHeight;
            ScaleTransform.ScaleX = scale;
            ScaleTransform.ScaleY = scale;

            // マウス状態を再評価
            foreach (var nodeContainer in NodeContainers)
                nodeContainer.RefreshMouseState();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.A:
                    FitAllNodes();
                    break;

                case Key.F:
                    FitSelectedNodes();
                    break;
            }
        }

        #endregion

        public Point ControlPosToNodeEditorPos(Point point)
        {
            return this.TransformPos(point.X, point.Y);
        }
    }
}