using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

    public class BiaNodeEditor : BiaClippingBorder, IHasTransform
    {
        #region NodesSource

        public IEnumerable NodesSource
        {
            get => _NodesSource;
            set
            {
                if (!Equals(value, _NodesSource))
                    SetValue(NodesSourceProperty, value);
            }
        }

        private IEnumerable _NodesSource;

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

        public IEnumerable LinksSource
        {
            get => _LinksSource;
            set
            {
                if (!Equals(value, _LinksSource))
                    SetValue(LinksSourceProperty, value);
            }
        }

        private IEnumerable _LinksSource;

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

        public IBiaNodeSlotEnabledChecker NodeSlotEnabledChecker
        {
            get => _nodeSlotEnabledChecker;
            set
            {
                if (value != _nodeSlotEnabledChecker)
                    SetValue(NodeSlotEnabledCheckerProperty, value);
            }
        }

        private IBiaNodeSlotEnabledChecker _nodeSlotEnabledChecker;

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

        public Color HighlightLinkColor
        {
            get => _HighlightLinkColor;
            set
            {
                if (value != _HighlightLinkColor)
                    SetValue(HighlightLinkColorProperty, value);
            }
        }

        private Color _HighlightLinkColor = Colors.GhostWhite;

        public static readonly DependencyProperty HighlightLinkColorProperty =
            DependencyProperty.Register(
                nameof(HighlightLinkColor),
                typeof(Color),
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Colors.GhostWhite,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._HighlightLinkColor = (Color) e.NewValue;
                    }));

        #endregion

        #region NodeLinkStyle

        public BiaNodeEditorNodeLinkStyle NodeLinkStyle
        {
            get => _NodeLinkStyle;
            set
            {
                if (value != _NodeLinkStyle)
                    SetValue(NodeLinkStyleProperty, value);
            }
        }

        private BiaNodeEditorNodeLinkStyle _NodeLinkStyle = BiaNodeEditorNodeLinkStyle.AxisAlign;

        public static readonly DependencyProperty NodeLinkStyleProperty =
            DependencyProperty.Register(nameof(NodeLinkStyle), typeof(BiaNodeEditorNodeLinkStyle),
                typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(
                    BiaNodeEditorNodeLinkStyle.AxisAlign,
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
                    SetValue(ScaleProperty, value);
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
                    }));

        #endregion

        #region CanConnectLink
        
        public bool CanConnectLink
        {
            get => _CanConnectLink;
            set
            {
                if (value != _CanConnectLink)
                    SetValue(CanConnectLinkProperty, value);
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
                        self._CanConnectLink = (bool)e.NewValue;
                    }));
        
        #endregion

        public event EventHandler<NodeLinkStartingEventArgs> NodeLinkStarting;

        public event EventHandler<NodeLinkCompletedEventArgs> NodeLinkCompleted;

        public event EventHandler PropertyEditStarting;

        public event EventHandler PropertyEditCompleted;

        public ScaleTransform ScaleTransform { get; } = new ScaleTransform();

        public TranslateTransform TranslateTransform { get; } = new TranslateTransform();

        public bool IsNodeSlotDragging { get; internal set; }

        internal event EventHandler NodeItemMoved;

        internal event EventHandler NodesSourceChanging;

        internal event EventHandler LinksSourceChanging;

        internal event EventHandler LinkChanged;

        static BiaNodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(typeof(BiaNodeEditor)));
        }

        private readonly NodeContainer _nodeContainer;

        public BiaNodeEditor()
        {
            var mouseOperator = new MouseOperator(this, this);

            _nodeContainer = new NodeContainer(this, mouseOperator);

            SetupPropertyEditCommand(mouseOperator);

            IBackgroundPanel backgroundPanel;
            var grid = new Grid();
            {
                backgroundPanel = BiauiEnvironment.BackgroundPanelGenerator.Generate(this);

                grid.Children.Add((UIElement) backgroundPanel);
                grid.Children.Add(_nodeContainer);
                grid.Children.Add(new BoxSelector(mouseOperator));
                grid.Children.Add(new NodeSlotConnector(this, mouseOperator));
                grid.Children.Add(CreateScaleSlider());
            }

            base.Child = grid;

            ScaleTransform.Changed += (_, __) => Scale = ScaleTransform.ScaleX;

            // backgroundPanel
            {
                TranslateTransform.Changed += (_, __) => backgroundPanel.Invalidate();
                ScaleTransform.Changed += (_, __) => backgroundPanel.Invalidate();
                NodeItemMoved += (_, __) => backgroundPanel.Invalidate();
                LinksSourceChanging += (_, __) => backgroundPanel.Invalidate();
                LinkChanged += (_, __) => backgroundPanel.Invalidate();
                mouseOperator.PreMouseLeftButtonUp += (_, __) => backgroundPanel.Invalidate();
            }
        }

        private void UpdateLinksSource(
            IEnumerable oldSource,
            IEnumerable newSource)
        {
            LinksSourceChanging?.Invoke(this, EventArgs.Empty);

            var observableOldSource = oldSource as INotifyCollectionChanged;
            var observableNewSource = newSource as INotifyCollectionChanged;

            if (observableOldSource != null)
                observableOldSource.CollectionChanged -= LinksSourceOnCollectionChanged;

            if (observableNewSource != null)
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
                Minimum = Constants.NodeEditor_MinScale,
                Maximum = Constants.NodeEditor_MaxScale,
                Value = ScaleTransform.ScaleX
            };

            scaleSlider.ValueChanged += (_, __) =>
            {
                var cx = ActualWidth * 0.5;
                var cy = ActualHeight * 0.5;

                this.SetTransform(scaleSlider.Value, cx, cy);
            };

            ScaleTransform.Changed += (_, __) => scaleSlider.Value = ScaleTransform.ScaleX;

            return scaleSlider;
        }

        private void SetupPropertyEditCommand(MouseOperator mouseOperator)
        {
            var isInEditing = false;

            mouseOperator.PostMouseLeftButtonDown += (_, __) =>
            {
                // キー操作を受け付けるためにフォーカスをあてる
                Focus();

                isInEditing = mouseOperator.IsBoxSelect ||
                              mouseOperator.IsPanelMove;

                if (isInEditing == false)
                    return;

                InvokePropertyEditStarting();
            };


            mouseOperator.PostMouseLeftButtonUp += (_, __) =>
            {
                if (isInEditing == false)
                    return;

                InvokePropertyEditCompleted();
                isInEditing = false;
            };
        }

        public void Sleep()
        {
            ++_nodeContainer.IsEnableUpdateChildrenBagDepth;
        }

        public void Wakeup()
        {
            --_nodeContainer.IsEnableUpdateChildrenBagDepth;
            Debug.Assert(_nodeContainer.IsEnableUpdateChildrenBagDepth >= 0);

            if (_nodeContainer.IsEnableUpdateChildrenBagDepth == 0)
                _nodeContainer.UpdateChildrenBag(true);
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

            var scaleX = ActualWidth / w;
            var scaleY = ActualHeight / h;

            var viewCx = ActualWidth * 0.5;
            var viewCy = ActualHeight * 0.5;

            var scale = (scaleX, scaleY).Min();
            scale = (scale, Constants.NodeEditor_MinScale, Constants.NodeEditor_MaxScale).Clamp();

            TranslateTransform.X = -centerX * scale + viewCx;
            TranslateTransform.Y = -centerY * scale + viewCy;
            ScaleTransform.ScaleX = scale;
            ScaleTransform.ScaleY = scale;
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