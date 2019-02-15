using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            grid.Children.Add(new BackgroundPanel(this, mouseOperator));
            grid.Children.Add(new NodeContainer(this, mouseOperator));
            grid.Children.Add(new BoxSelector(mouseOperator));
            grid.Children.Add(new NodePortConnector(this, mouseOperator));
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
    }
}