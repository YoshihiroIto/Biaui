using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class NodeContainer : FrameworkElementBag<BiaNodePanel>
    {
        #region NodesSource

        internal IEnumerable NodesSource
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
                typeof(NodeContainer),
                new PropertyMetadata(
                    default(IEnumerable),
                    (s, e) =>
                    {
                        var self = (NodeContainer) s;

                        var old = self._NodesSource;
                        self._NodesSource = (IEnumerable) e.NewValue;
                        self.UpdateNodesSource(old as INotifyCollectionChanged, self._NodesSource as INotifyCollectionChanged);
                    }));

        #endregion

        private readonly BiaNodeEditor _parent;
        private readonly MouseOperator _mouseOperator;

        private readonly Dictionary<IBiaNodeItem, BiaNodePanel> _nodeDict
            = new Dictionary<IBiaNodeItem, BiaNodePanel>();

        private readonly List<(IBiaNodeItem, BiaNodePanel)> _changedUpdate
            = new List<(IBiaNodeItem, BiaNodePanel)>();

        private readonly Stack<BiaNodePanel> _removeNodePanelPool = new Stack<BiaNodePanel>();
        private readonly Stack<BiaNodePanel> _recycleNodePanelPool = new Stack<BiaNodePanel>();
        private readonly HashSet<IBiaNodeItem> _selectedNodes = new HashSet<IBiaNodeItem>();
        private readonly HashSet<IBiaNodeItem> _preSelectedNodes = new HashSet<IBiaNodeItem>();

        private readonly DispatcherTimer _removeNodePanelTimer;

        private int _isEnableUpdateChildrenBagDepth;

        internal NodeContainer(BiaNodeEditor parent, MouseOperator mouseOperator)
            : base(parent)
        {
            _parent = parent;
            _mouseOperator = mouseOperator;

            _parent.SizeChanged += (_, __) => UpdateChildrenBag(true);
            _parent.Unloaded += (_, __) => StopTimer();
            _parent.NodesSourceChanging += (_, __) => Clear();

            _mouseOperator.PanelMoving += OnPanelMoving;
            _mouseOperator.PostMouseWheel += OnPostMouseWheel;
            _mouseOperator.PostMouseMove += OnPostMouseMove;
            _mouseOperator.PreMouseLeftButtonUp += OnPreMouseLeftButtonUp;

            _removeNodePanelTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(1000),
                DispatcherPriority.ApplicationIdle,
                (_, __) => DoRemoverNodePanel(),
                Dispatcher.CurrentDispatcher);

            StopTimer();

            SetBinding(
                NodesSourceProperty,
                new Binding(nameof(NodesSource))
                {
                    Source = parent,
                    Mode = BindingMode.OneWay
                });
        }

        private void OnPanelMoving(object sender, MouseOperator.PanelMovingEventArgs e)
        {
            ++_isEnableUpdateChildrenBagDepth;
            {
                foreach (var n in _selectedNodes)
                    n.Pos += e.Diff;
            }
            --_isEnableUpdateChildrenBagDepth;
            Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
        }

        private void OnPreMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseOperator.IsBoxSelect)
            {
                SelectNodes(_parent.TransformRect(_mouseOperator.SelectionRect));
                ClearPreSelectedNode();
            }

            if (_mouseOperator.IsPanelMove)
                if (_mouseOperator.IsMoved)
                    AlignSelectedNodes();

            UpdateChildrenBag(true);

            if (_parent.IsNodePortDragging)
            {
                if (_parent.TargetNodePortConnecting.IsNotNull)
                {
                    _parent.InvokeNodeLinkCompleted(
                        new NodeLinkCompletedEventArgs(
                            _parent.SourceNodePortConnecting.ToItemPortIdPair(),
                            _parent.TargetNodePortConnecting.ToItemPortIdPair()));
                }

                UpdateNodePortEnabled(false);
            }

            _parent.SourceNodePortConnecting = default;
            _parent.TargetNodePortConnecting = default;
        }

        private void OnPostMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseOperator.IsOperating)
                UpdateChildrenBag(false);

            if (_mouseOperator.IsBoxSelect)
                PreSelectNodes(_parent.TransformRect(_mouseOperator.SelectionRect));
        }

        private void OnPostMouseWheel(object sender, MouseWheelEventArgs e)
        {
            UpdateChildrenBag(true);
        }

        private void StopTimer()
        {
            _removeNodePanelTimer.Stop();
        }

        private void AddOrUpdate(IBiaNodeItem nodeItem, BiaNodePanel panel)
        {
            _nodeDict[nodeItem] = panel;
        }

        private void Remove(IBiaNodeItem nodeItem)
        {
            _nodeDict.Remove(nodeItem);
            _selectedNodes.Remove(nodeItem);
            _preSelectedNodes.Remove(nodeItem);
        }

        private void Clear()
        {
            foreach (var i in _nodeDict)
            {
                var nodeItem = i.Key;
                var panel = i.Value;

                nodeItem.PropertyChanged -= NodeItemPropertyChanged;

                if (panel != null)
                    RemoveNodePanel(panel);
            }

            _nodeDict.Clear();
            _selectedNodes.Clear();
            _preSelectedNodes.Clear();
        }

        private void ClearPreSelectedNode()
        {
            foreach (var n in _preSelectedNodes.ToArray())
                n.IsPreSelected = false;
        }

        private void SelectNodes(in ImmutableRect rect)
        {
            ++_isEnableUpdateChildrenBagDepth;
            {
                // [Ctrl]押下で追加する
                if (KeyboardHelper.IsPressControl == false)
                    ClearSelectedNode();

                foreach (var c in Children)
                {
                    var node = (IBiaNodeItem) c.DataContext;

                    if (HitTest(node, rect) == false)
                        continue;

                    if (KeyboardHelper.IsPressControl)
                        node.IsSelected = !node.IsSelected;
                    else
                        node.IsSelected = true;
                }
            }
            --_isEnableUpdateChildrenBagDepth;
            Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
        }

        private void PreSelectNodes(in ImmutableRect rect)
        {
            ++_isEnableUpdateChildrenBagDepth;
            {
                ClearPreSelectedNode();

                foreach (var c in Children)
                {
                    var node = (IBiaNodeItem) c.DataContext;

                    if (HitTest(node, rect) == false)
                        continue;

                    node.IsPreSelected = true;
                }
            }
            --_isEnableUpdateChildrenBagDepth;
            Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
        }

        private void AlignSelectedNodes()
        {
            ++_isEnableUpdateChildrenBagDepth;

            foreach (var n in _selectedNodes)
                n.Pos = n.AlignedPos();

            --_isEnableUpdateChildrenBagDepth;
            Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
        }

        private bool HitTest(IBiaNodeItem node, in ImmutableRect rect)
        {
            switch (node.HitType)
            {
                case BiaNodePanelHitType.Rectangle:
                    if (rect.IntersectsWith(node.MakeRect()) == false)
                        return false;

                    break;

                case BiaNodePanelHitType.Circle:
                    if (rect.IntersectsWith(node.MakeCircle()) == false)
                        return false;

                    break;

                case BiaNodePanelHitType.Visual:
                    // まずは矩形で判断する
                    if (rect.IntersectsWith(node.MakeRect()) == false)
                        return false;

                    var panel = FindPanel(node);
                    if (panel == null)
                        return false;

                    if (IsHitVisual(rect, node, panel) == false)
                        return false;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        private readonly RectangleGeometry _rectGeom = new RectangleGeometry();

        private bool IsHitVisual(in ImmutableRect rect, IBiaNodeItem node, Visual panel)
        {
            _rectGeom.Rect = new Rect(
                rect.X - node.Pos.X,
                rect.Y - node.Pos.Y,
                rect.Width,
                rect.Height);

            var isHit = false;

            VisualTreeHelper.HitTest(
                panel,
                null,
                r =>
                {
                    isHit = true;
                    return HitTestResultBehavior.Stop;
                },
                new GeometryHitTestParameters(_rectGeom));

            return isHit;
        }

        private void ReturnNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _recycleNodePanelPool.Push(panel);
        }

        private void RemoveNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            ReturnNodePanel(panel);
            RemoveChild(panel);
        }

        private void DoRemoverNodePanel()
        {
            for (var i = 0; i != 5; ++i)
            {
                if (_removeNodePanelPool.Count == 0)
                    break;

                var p = _removeNodePanelPool.Pop();

                RemoveNodePanel(p);
            }

            if (_removeNodePanelPool.Count == 0)
                _removeNodePanelTimer.Stop();
        }

        private void RequestRemoveNodePanel(BiaNodePanel panel)
        {
            _removeNodePanelPool.Push(panel);

            _removeNodePanelTimer.Start();
        }

        private BiaNodePanel FindPanel(IBiaNodeItem nodeItem)
        {
            return _nodeDict.TryGetValue(nodeItem, out var panel) == false
                ? null
                : panel;
        }

        public void UpdateChildrenBag(bool isPushRemove)
        {
            if (_isEnableUpdateChildrenBagDepth > 0)
                return;

            var viewPortRect = _parent.TransformRect(_parent.ActualWidth, _parent.ActualHeight);

            UpdateChildrenBag(viewPortRect, isPushRemove);

            InvalidateMeasure();
        }

        private void UpdateChildrenBag(in ImmutableRect viewportRect, bool isPushRemove)
        {
            // メモ：
            // 一見、以降のループ内でitem.SizeのSetterを呼び出し変更通知経由でメソッドに再入するように見えるが、
            // 対象のitemはまだ_nodeDictに登録されていないので問題ない(再入しない)。

            var enabledCheckerArgs = new BiaNodePortEnabledCheckerArgs(
                BiaNodePortEnableTiming.Default,
                new BiaNodeItemPortIdPair(null, 0));

            foreach (var c in _nodeDict)
            {
                var nodeItem = c.Key;
                var nodePanel = c.Value;

                Type nodeItemType = null;

                ImmutableRect itemRect;
                {
                    if (nodeItem.Size == default)
                    {
                        var pos = nodeItem.Pos;

                        nodeItemType = nodeItem.GetType();

                        // 同タイプのパネルが表示済みならその大きさを使う
                        if (_panelDefaultSizeDict.TryGetValue(nodeItemType, out var size))
                        {
                            itemRect = new ImmutableRect(pos.X, pos.Y, size.Width, size.Height);
                            nodeItem.Size = size;
                        }
                        else
                        {
                            // 対象アイテムが一度も表示されていない場合は、大きさを適当に設定してしのぐ
                            const double tempWidth = 256.0;
                            const double tempHeight = 512.0;

                            itemRect = new ImmutableRect(pos.X, pos.Y, tempWidth, tempHeight);
                        }
                    }
                    else
                    {
                        var pos = nodeItem.Pos;
                        var size = nodeItem.Size;
                        itemRect = new ImmutableRect(pos.X, pos.Y, size.Width, size.Height);
                    }
                }

                if (viewportRect.IntersectsWith(itemRect))
                {
                    if (nodePanel == null)
                    {
                        bool isAdded;
                        (nodePanel, isAdded) = FindOrCreateNodePanel();

                        var style = nodeItem.InternalData().Style;
                        if (style == null)
                        {
                            if (nodeItemType == null)
                                nodeItemType = nodeItem.GetType();

                            if (_styleDict.TryGetValue(nodeItemType, out style) == false)
                            {
                                style = FindResource(nodeItemType) as Style;
                                _styleDict.Add(nodeItemType, style);
                            }

                            nodeItem.InternalData().Style = style;
                        }

                        nodePanel.Style = style;
                        nodePanel.DataContext = nodeItem;
                        nodePanel.Opacity = 1.0;

                        UpdateNodePortEnabled(nodeItem, enabledCheckerArgs);

                        _changedUpdate.Add((nodeItem, nodePanel));

                        if (isAdded)
                            ChangeElement(nodePanel);
                        else
                            AddChild(nodePanel);
                    }
                }
                else
                {
                    if (isPushRemove)
                    {
                        if (nodePanel != null)
                        {
                            RequestRemoveNodePanel(nodePanel);

                            _changedUpdate.Add((nodeItem, null));
                        }
                    }
                }
            }

            foreach (var c in _changedUpdate)
                AddOrUpdate(c.Item1, c.Item2);

            _changedUpdate.Clear();
        }

        private (BiaNodePanel Panel, bool IsAdded) FindOrCreateNodePanel()
        {
            // 削除候補から見つかれば、それを優先して返す。
            // 返却候補はまだ、追加済み。

            if (_removeNodePanelPool.Count != 0)
                return (_removeNodePanelPool.Pop(), true);

            if (_recycleNodePanelPool.Count != 0)
                return (_recycleNodePanelPool.Pop(), false);

            var p = new BiaNodePanel();

            p.MouseEnter += NodePanel_OnMouseEnter;
            p.MouseLeave += NodePanel_OnMouseLeave;
            p.MouseLeftButtonDown += NodePanel_OnMouseLeftButtonDown;
            p.MouseMove += NodePanel_OnMouseMove;

            return (p, false);
        }

        private void ClearSelectedNode()
        {
            foreach (var n in _selectedNodes.ToArray())
                n.IsSelected = false;
        }

        private void UpdateNodePortEnabled(bool isStart)
        {
            if (_parent.NodePortEnabledChecker == null)
                return;

            var args = new BiaNodePortEnabledCheckerArgs(
                isStart
                    ? BiaNodePortEnableTiming.ConnectionStarting
                    : BiaNodePortEnableTiming.Default,
                new BiaNodeItemPortIdPair(
                    _parent.SourceNodePortConnecting.Item,
                    _parent.SourceNodePortConnecting.Port.Id
                ));

            foreach (var child in Children)
            {
                var nodeItem = (IBiaNodeItem) child.DataContext;

                UpdateNodePortEnabled(nodeItem, args);

                child.InvalidatePorts();

                if (isStart)
                    child.Opacity = nodeItem.InternalData().EnablePorts.Count > 0
                        ? 1.0
                        : 0.2;
                else
                    child.Opacity = 1.0;
            }
        }

        private void UpdateNodePortEnabled(IBiaNodeItem target, in BiaNodePortEnabledCheckerArgs args)
        {
            target.InternalData().EnablePorts.Clear();

            if (_parent.NodePortEnabledChecker == null)
                return;

            foreach (var enabledPortId in _parent.NodePortEnabledChecker.Check(target, args))
            {
                Debug.Assert(target.Layout.Ports.ContainsKey(enabledPortId));

                var port = target.Layout.Ports[enabledPortId];
                target.InternalData().EnablePorts.Add(port);
            }
        }

        private void UpdateSelectedNode(IBiaNodeItem node)
        {
            if (node.IsSelected)
                _selectedNodes.Add(node);
            else
                _selectedNodes.Remove(node);

            if (node.IsPreSelected)
                _preSelectedNodes.Add(node);
            else
                _preSelectedNodes.Remove(node);
        }

        private void UpdateNodesSource(
            INotifyCollectionChanged oldSource,
            INotifyCollectionChanged newSource)
        {
            if (oldSource != null)
                oldSource.CollectionChanged -= NodesSourceOnCollectionChanged;

            if (newSource != null)
            {
                newSource.CollectionChanged += NodesSourceOnCollectionChanged;

                // 最初は全部追加として扱う
                NodesSourceOnCollectionChanged(null,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)newSource, 0));
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

                        AddOrUpdate(nodeItem, null);
                        UpdateSelectedNode(nodeItem);
                    }

                    UpdateChildrenBag(true);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null)
                        break;

                    foreach (IBiaNodeItem nodeItem in e.OldItems)
                    {
                        nodeItem.PropertyChanged -= NodeItemPropertyChanged;

                        var panel = FindPanel(nodeItem);
                        if (panel != null)
                            RemoveNodePanel(panel);

                        Remove(nodeItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
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

                        var panel = FindPanel(oldItem);

                        oldItem.PropertyChanged -= NodeItemPropertyChanged;
                        Remove(oldItem);

                        newItem.PropertyChanged += NodeItemPropertyChanged;
                        AddOrUpdate(newItem, null);
                        UpdateSelectedNode(newItem);

                        if (panel != null)
                            RemoveNodePanel(panel);
                    }

                    UpdateChildrenBag(true);
                    break;
                }

                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NodeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (IBiaNodeItem) sender;

            switch (e.PropertyName)
            {
                case nameof(IBiaNodeItem.Pos):
                case nameof(IBiaNodeItem.Size):
                {
                    _parent.InvokeNodeItemMoved();

                    ChangeElementInternal(true);
                    break;
                }

                case nameof(IBiaNodeItem.IsSelected):
                case nameof(IBiaNodeItem.IsPreSelected):
                {
                    UpdateSelectedNode(node);
                    ChangeElementInternal(false);
                    break;
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////
            void ChangeElementInternal(bool isPushRemove)
            {
                var panel = FindPanel(node);

                if (panel != null)
                {
                    ChangeElement(panel);
                    UpdateChildrenBag(isPushRemove);
                }
            }
        }

        #region NodePanel

        private void NodePanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var panel = (BiaNodePanel) sender;
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            ToLast(panel);

            nodeItem.IsMouseOver = true;

            e.Handled = true;
        }

        private void NodePanel_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var panel = (BiaNodePanel) sender;
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            nodeItem.IsMouseOver = false;

            panel.InvalidatePorts();

            e.Handled = true;
        }

        private void NodePanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var panel = (BiaNodePanel) sender;
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            if (nodeItem.IsSelected == false)
            {
                // [Ctrl]押下で追加する
                if (KeyboardHelper.IsPressControl == false)
                {
                    ClearSelectedNode();

                    nodeItem.IsSelected = true;
                }
                else
                    nodeItem.IsSelected = !nodeItem.IsSelected;
            }

            var port = nodeItem.FindPortFromPos(e.GetPosition(panel));

            _mouseOperator.OnMouseLeftButtonDown(
                e,
                port == null
                    ? MouseOperator.TargetType.NodePanel
                    : MouseOperator.TargetType.NodeLink);

            if (_mouseOperator.IsLinkMove)
            {
                if (port == null)
                    throw new NotSupportedException();

                var args = new NodeLinkStartingEventArgs(new BiaNodeItemPortIdPair(nodeItem, port.Id));
                _parent.InvokeNodeLinkStarting(args);

                _parent.SourceNodePortConnecting = new BiaNodeItemPortPair(nodeItem, port);
                _parent.TargetNodePortConnecting = default;

                UpdateNodePortEnabled(true);
            }

            e.Handled = true;
        }

        private void NodePanel_OnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseOperator.OnMouseMove(e);
        }

        #endregion

        protected override void ArrangeChildren(IEnumerable<BiaNodePanel> children)
        {
            foreach (var child in children)
            {
                var pos = ((IBiaHasPos) child.DataContext).AlignedPos();

                child.Arrange(new Rect(pos, child.DesiredSize));
            }
        }

        protected override void MeasureChildren(IEnumerable<BiaNodePanel> children, Size availableSize)
        {
            foreach (var child in children)
            {
                child.Measure(availableSize);

                var nodeItem = (IBiaNodeItem)child.DataContext;

                var desiredSize = child.DesiredSize;

                if (nodeItem.Size == default)
                {
                    var nodeItemType = nodeItem.GetType();

                    if (_panelDefaultSizeDict.ContainsKey(nodeItemType) == false)
                        _panelDefaultSizeDict.Add(nodeItemType, desiredSize);
                }

                nodeItem.Size = desiredSize;
            }
        }

        private readonly Dictionary<Type, Style> _styleDict = new Dictionary<Type, Style>();
        private readonly Dictionary<Type, Size> _panelDefaultSizeDict = new Dictionary<Type, Size>();
    }
}