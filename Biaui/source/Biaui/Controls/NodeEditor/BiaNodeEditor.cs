using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeEditor : BiaClippingBorder, IHasTransform, IHasIsNodePortDragging
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

                        var old = self._NodesSource;
                        self._NodesSource = (ObservableCollection<IBiaNodeItem>) e.NewValue;
                        self.UpdateNodesSource(old, self._NodesSource);
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

        private readonly NodeContainer _nodeContainer;
        internal readonly BackgroundPanel _backgroundPanel;
        internal readonly FrameworkElementBag<BiaNodePanel> _nodePanelBag;

        static BiaNodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(typeof(BiaNodeEditor)));
        }

        public BiaNodeEditor()
        {
            SizeChanged += (_, __) => _nodeContainer.UpdateChildrenBag(true);
            Unloaded += (_, __) => _nodeContainer.Stop();

            var mouseOperator = new MouseOperator(this, this);

            _nodeContainer = new NodeContainer(this, mouseOperator);

            var grid = new Grid();
            grid.Children.Add(_backgroundPanel = new BackgroundPanel(this, this, mouseOperator));
            grid.Children.Add(_nodePanelBag = new FrameworkElementBag<BiaNodePanel>(this));
            grid.Children.Add(new BoxSelector(mouseOperator));
            grid.Children.Add(new NodePortConnector(this, mouseOperator));
            base.Child = grid;

            _backgroundPanel.SetBinding(BackgroundPanel.LinksSourceProperty, new Binding(nameof(LinksSource))
            {
                Source = this,
                Mode = BindingMode.OneWay
            });
        }

        #region Nodes

        private void UpdateNodesSource(
            ObservableCollection<IBiaNodeItem> oldSource,
            ObservableCollection<IBiaNodeItem> newSource)
        {
            if (oldSource != null)
            {
                _nodeContainer.CleanAll();

                oldSource.CollectionChanged -= NodesSourceOnCollectionChanged;
            }

            if (newSource != null)
            {
                newSource.CollectionChanged += NodesSourceOnCollectionChanged;

                // 最初は全部追加として扱う
                NodesSourceOnCollectionChanged(null,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newSource, 0));
            }
        }

        internal void NodeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (IBiaNodeItem) sender;

            switch (e.PropertyName)
            {
                case nameof(IBiaNodeItem.Pos):
                case nameof(IBiaNodeItem.Size):
                {
                    ChangeElement(true);
                    _backgroundPanel.InvalidateVisual();
                    break;
                }

                case nameof(IBiaNodeItem.IsSelected):
                case nameof(IBiaNodeItem.IsPreSelected):
                {
                    _nodeContainer.UpdateSelectedNode(node);
                    ChangeElement(false);
                    break;
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////
            void ChangeElement(bool isPushRemove)
            {
                var panel = _nodeContainer.FindPanel(node);

                if (panel != null)
                {
                    _nodePanelBag.ChangeElement(panel);
                    _nodeContainer.UpdateChildrenBag(isPushRemove);
                }
            }
        }

        private void NodesSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null)
                        break;

                    foreach (IBiaNodeItem nodeItem in e.NewItems)
                    {
                        nodeItem.PropertyChanged += NodeItemPropertyChanged;

                        _nodeContainer.AddOrUpdate(nodeItem, null);
                        _nodeContainer.UpdateSelectedNode(nodeItem);
                    }

                    _nodeContainer.UpdateChildrenBag(true);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null)
                        break;

                    foreach (IBiaNodeItem nodeItem in e.OldItems)
                    {
                        nodeItem.PropertyChanged -= NodeItemPropertyChanged;

                        var panel = _nodeContainer.FindPanel(nodeItem);
                        if (panel != null)
                            _nodeContainer.RemoveNodePanel(panel);

                        _nodeContainer.Remove(nodeItem);
                    }

                    _backgroundPanel.InvalidateVisual();

                    break;

                case NotifyCollectionChangedAction.Reset:
                    _nodeContainer.CleanAll();
                    break;

                case NotifyCollectionChangedAction.Replace:
                {
                    var oldItems = e.OldItems.Cast<IBiaNodeItem>().ToArray();
                    var newItems = e.NewItems.Cast<IBiaNodeItem>().ToArray();

                    if (oldItems.Length != newItems.Length)
                        throw new NotSupportedException();

                    for (var i = 0; i != oldItems.Length; ++i)
                    {
                        var oldItem = oldItems[i];
                        var newItem = newItems[i];

                        var panel = _nodeContainer.FindPanel(oldItem);

                        oldItem.PropertyChanged -= NodeItemPropertyChanged;
                        _nodeContainer.Remove(oldItem);

                        newItem.PropertyChanged += NodeItemPropertyChanged;
                        _nodeContainer.AddOrUpdate(newItem, null);
                        _nodeContainer.UpdateSelectedNode(newItem);

                        if (panel != null)
                            _nodeContainer.RemoveNodePanel(panel);
                    }

                    _nodeContainer.UpdateChildrenBag(true);

                    _backgroundPanel.InvalidateVisual();

                    break;
                }

                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        private void UpdateLinksSource(
            ObservableCollection<IBiaNodeLink> oldSource,
            ObservableCollection<IBiaNodeLink> newSource)
        {
            if (oldSource != null)
                oldSource.CollectionChanged -= LinksSourceOnCollectionChanged;

            if (newSource != null)
                newSource.CollectionChanged += LinksSourceOnCollectionChanged;

            //////////////////////////////////////////////////////////////////////////////
            void LinksSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _backgroundPanel.InvalidateVisual();
            }
        }

        internal void InvokeNodeLinkCompleted(NodeLinkCompletedEventArgs args)
            => NodeLinkCompleted?.Invoke(this, args);

        internal void InvokeNodeLinkStarting(NodeLinkStartingEventArgs args)
            => NodeLinkStarting?.Invoke(this, args);
    }
}