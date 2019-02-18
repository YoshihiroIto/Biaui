using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
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

        public event EventHandler<NodeLinkStartingEventArgs> NodeLinkStarting;

        public event EventHandler<NodeLinkCompletedEventArgs> NodeLinkCompleted;

        public event EventHandler PropertyEditStarting;
        public event EventHandler PropertyEditCompleted;

        public ScaleTransform Scale { get; } = new ScaleTransform();

        public TranslateTransform Translate { get; } = new TranslateTransform();

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

        public BiaNodeEditor()
        {
            var mouseOperator = new MouseOperator(this, this);

            SetupPropertyEditCommand(mouseOperator);

            var grid = new Grid();
            {
                grid.Children.Add(new BackgroundPanel(this, mouseOperator));
                grid.Children.Add(new NodeContainer(this, mouseOperator));
                grid.Children.Add(new BoxSelector(mouseOperator));
                grid.Children.Add(new NodeSlotConnector(this, mouseOperator));
                grid.Children.Add(CreateScaleSlider());
            }

            base.Child = grid;
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
                Value = Scale.ScaleX
            };

            scaleSlider.ValueChanged += (_, __) =>
            {
                var cx = ActualWidth * 0.5;
                var cy = ActualHeight * 0.5;

                this.SetTransform(scaleSlider.Value, cx, cy);
            };

            Scale.Changed += (_, __) => scaleSlider.Value = Scale.ScaleX;

            return scaleSlider;
        }

        private void SetupPropertyEditCommand(MouseOperator mouseOperator)
        {
            var isInEditing = false;

            mouseOperator.PrePreviewMouseLeftButtonDown += (_, __) =>
            {
                Debug.Assert(isInEditing == false);

                PropertyEditStarting?.Invoke(this, EventArgs.Empty);
                isInEditing = true;
            };

            mouseOperator.PostMouseLeftButtonUp += (_, __) =>
            {
                if (isInEditing == false)
                    return;

                PropertyEditCompleted?.Invoke(this, EventArgs.Empty);
                isInEditing = false;
            };
        }
    }
}