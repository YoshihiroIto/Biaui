using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Memory;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class NodeContainer : FrameworkElementBag<BiaNodePanel>
    {
        #region NodesSource

        internal IEnumerable? NodesSource
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
                typeof(NodeContainer),
                new PropertyMetadata(
                    default(IEnumerable),
                    (s, e) =>
                    {
                        var self = (NodeContainer) s;

                        var old = self._NodesSource;
                        self._NodesSource = (IEnumerable) e.NewValue;
                        self.UpdateNodesSource(old as INotifyCollectionChanged,
                            self._NodesSource as INotifyCollectionChanged);
                    }));

        #endregion

        private readonly BiaNodeEditor _parent;
        private readonly BiaNodePanelLayer _layer;
        private readonly MouseOperator _mouseOperator;

        private readonly Dictionary<IBiaNodeItem, BiaNodePanel?> _nodeDict = new Dictionary<IBiaNodeItem, BiaNodePanel?>();

        private readonly Stack<BiaNodePanel> _recycleNodePanelPool = new Stack<BiaNodePanel>();
        private readonly HashSet<IBiaNodeItem> _selectedNodes = new HashSet<IBiaNodeItem>();
        private readonly HashSet<IBiaNodeItem> _preSelectedNodes = new HashSet<IBiaNodeItem>();

        internal IEnumerable<IBiaNodeItem> SelectedNodes => _selectedNodes;
        internal IEnumerable<IBiaNodeItem> PreSelectedNodes => _preSelectedNodes;

        internal int IsEnableUpdateChildrenBagDepth;

        internal NodeContainer(BiaNodeEditor parent, BiaNodePanelLayer layer, MouseOperator mouseOperator)
            : base(parent)
        {
            _parent = parent;
            _layer = layer;
            _mouseOperator = mouseOperator;

            _parent.SizeChanged += (_, __) => UpdateChildrenBag();
            _parent.NodesSourceChanging += (_, __) =>
            {
                Clear();
                _parent.FitAllNodes();
            };
            _parent.ScaleTransform.Changed += (_, __) => UpdateChildrenBag();
            _parent.TranslateTransform.Changed += (_, __) => UpdateChildrenBag();

            _mouseOperator.PanelBeginMoving += OnPanelBeginMoving;
            _mouseOperator.PanelEndMoving += OnPanelEndMoving;
            _mouseOperator.PanelMoving += OnPanelMoving;
            _mouseOperator.PostMouseMove += OnPostMouseMove;
            _mouseOperator.PreMouseLeftButtonUp += OnPreMouseLeftButtonUp;

            SetBinding(
                NodesSourceProperty,
                new Binding(nameof(NodesSource))
                {
                    Source = _parent,
                    Mode = BindingMode.OneWay
                });
        }

        private CanMoveByDraggingArgs? _CanMoveByDraggingArgs;

        private void OnPanelBeginMoving(object? sender, EventArgs e)
        {
            _CanMoveByDraggingArgs = new CanMoveByDraggingArgs(_parent.NodeContainers.SelectMany(x => x.SelectedNodes));
        }

        private void OnPanelEndMoving(object? sender, EventArgs e)
        {
            _CanMoveByDraggingArgs = null;
        }

        private void OnPanelMoving(object? sender, MouseOperator.PanelMovingEventArgs e)
        {
            ++IsEnableUpdateChildrenBagDepth;
            {
                foreach (var n in _selectedNodes)
                {
                    if (n.CanMoveByDragging(_CanMoveByDraggingArgs ?? throw new NullReferenceException()) == false)
                        continue;

                    n.Pos += e.Diff;
                }
            }
            --IsEnableUpdateChildrenBagDepth;
            Debug.Assert(IsEnableUpdateChildrenBagDepth >= 0);
        }

        private void OnPreMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_layer == BiaNodePanelLayer.Low)
            {
                if (_mouseOperator.IsBoxSelect)
                {
                    if (_mouseOperator.SelectionRect.HasArea == false)
                    {
                        // クリック（面積のないボックス選択）は何もしない
                    }
                    else
                    {
                        SelectNodes(_parent.TransformRect(_mouseOperator.SelectionRect));
                        ClearPreSelectedNodes();
                    }
                }

                if (_mouseOperator.IsPanelMove)
                    if (_mouseOperator.IsMoved)
                        AlignSelectedNodes();
            }

            UpdateChildrenBag();

            if (_parent.IsNodeSlotDragging)
            {
                if (_parent.TargetNodeSlotConnecting.IsNotNull)
                {
                    _parent.InvokeNodeLinkCompleted(
                        new NodeLinkCompletedEventArgs(
                            _parent.SourceNodeSlotConnecting.ToItemSlotIdPair(),
                            _parent.TargetNodeSlotConnecting.ToItemSlotIdPair()));
                }

                UpdateNodeSlotEnabled(false);
            }

            _parent.SourceNodeSlotConnecting = default;
            _parent.TargetNodeSlotConnecting = default;
        }

        private void OnPostMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseOperator.IsOperating)
                UpdateChildrenBag();

            if (_mouseOperator.IsBoxSelect)
                PreSelectNodes(_parent.TransformRect(_mouseOperator.SelectionRect));
        }

        private void AddOrUpdate(IBiaNodeItem nodeItem, BiaNodePanel? panel)
        {
            if (nodeItem.Layer != _layer)
                return;

            _nodeDict[nodeItem] = panel;
        }

        private void Remove(IBiaNodeItem nodeItem)
        {
            if (nodeItem.Layer != _layer)
                return;

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

        private void ClearPreSelectedNodes()
        {
            using var preSelectedNodes = _parent.NodeContainers.SelectMany(x => x.PreSelectedNodes).ToTempBuffer(256);

            foreach (var n in preSelectedNodes.Buffer)
                n.IsPreSelected = false;
        }

        private void SelectNodes(in ImmutableRect_double rect)
        {
            ++IsEnableUpdateChildrenBagDepth;
            {
                // [Ctrl]押下で追加する
                if (KeyboardHelper.IsPressControl == false)
                    ClearSelectedNodes();

                using var children = _parent.NodeContainers.SelectMany(x => x.Children).ToTempBuffer(256);

                foreach (var child in children.Buffer)
                {
                    if (child.IsActive == false)
                        continue;

                    var node = (IBiaNodeItem) child.DataContext;

                    if (HitTest(node, rect) == false)
                        continue;

                    if (KeyboardHelper.IsPressControl)
                        node.IsSelected = !node.IsSelected;
                    else
                        node.IsSelected = true;
                }
            }
            --IsEnableUpdateChildrenBagDepth;
            Debug.Assert(IsEnableUpdateChildrenBagDepth >= 0);
        }

        private void PreSelectNodes(in ImmutableRect_double rect)
        {
            ++IsEnableUpdateChildrenBagDepth;
            {
                foreach (var child in Children)
                {
                    if (child.IsActive == false)
                        continue;

                    var node = (IBiaNodeItem) child.DataContext;

                    node.IsPreSelected = HitTest(node, rect);
                }
            }
            --IsEnableUpdateChildrenBagDepth;

            Debug.Assert(IsEnableUpdateChildrenBagDepth >= 0);
        }

        private void AlignSelectedNodes()
        {
            ++IsEnableUpdateChildrenBagDepth;

            foreach (var n in _parent.NodeContainers.SelectMany(x => x.SelectedNodes))
                n.Pos = n.AlignPos();

            --IsEnableUpdateChildrenBagDepth;
            Debug.Assert(IsEnableUpdateChildrenBagDepth >= 0);
        }

        private bool HitTest(IBiaNodeItem node, in ImmutableRect_double rect)
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

        private bool IsHitVisual(in ImmutableRect_double rect, IBiaNodeItem node, Visual panel)
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
                    if (r.VisualHit is UIElement uiElement && uiElement.IsHitTestVisible)
                    {
                        isHit = true;
                        return HitTestResultBehavior.Stop;
                    }

                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(_rectGeom));

            return isHit;
        }

        private void RemoveNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            ReturnNodePanel(panel);
            RemoveChild(panel);
        }

        private void ReturnNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            panel.DataContext = null;

            _recycleNodePanelPool.Push(panel);
        }

        private BiaNodePanel? FindPanel(IBiaNodeItem nodeItem)
        {
            return _nodeDict.TryGetValue(nodeItem, out var panel) == false
                ? null
                : panel;
        }

        internal void UpdateChildrenBag()
        {
            if (IsEnableUpdateChildrenBagDepth > 0)
                return;

            var viewportRect = _parent.TransformRect(_parent.ActualWidth, _parent.ActualHeight);

            UpdateChildrenBag(viewportRect);

            InvalidateMeasure();

            foreach (var child in Children)
            {
                if (child.IsActive == false)
                    continue;

                child.InvalidateMeasure();
            }
        }

        internal void RefreshMouseState()
        {
            using var children = Children.ToTempBuffer();

            for (var i = 0; i != children.Length; ++i)
            {
                var child = children[i];
                
                if (child.IsActive == false)
                    continue;

                var nodeItem = (IBiaNodeItem) child.DataContext;

                nodeItem.IsMouseOver = false;
            }
        }

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        private void UpdateChildrenBag(in ImmutableRect_double viewportRect)
        {
            using var changedUpdate = new TempBuffer<(IBiaNodeItem, BiaNodePanel?)>(_nodeDict.Count);

            // メモ：
            // 一見、以降のループ内でitem.SizeのSetterを呼び出し変更通知経由でメソッドに再入するように見えるが、
            // 対象のitemはまだ_nodeDictに登録されていないので問題ない(再入しない)。
            foreach (var c in _nodeDict)
            {
                var nodeItem = c.Key;
                var nodePanel = c.Value;

                Type? nodeItemType = null;

                var isNoViewportCulling = nodeItem.Flags.HasFlag(BiaNodePaneFlags.NoViewportCulling);
                var itemRect = default(ImmutableRect_double);
                
                if (isNoViewportCulling == false)
                {
                    if (nodeItem.Size == default)
                    {
                        nodeItemType = nodeItem.GetType();

                        // 同タイプのパネルが表示済みならその大きさを使う
                        if (_panelDefaultSizeDict.TryGetValue(nodeItemType, out var size))
                        {
                            itemRect = new ImmutableRect_double(nodeItem.Pos.X, nodeItem.Pos.Y, size.Width, size.Height);
                        }
                        else
                        {
                            // 対象アイテムが一度も表示されていない場合は、大きさを適当に設定してしのぐ
                            const double tempWidth = 256.0;
                            const double tempHeight = 512.0;

                            itemRect = new ImmutableRect_double(nodeItem.Pos.X, nodeItem.Pos.Y, tempWidth, tempHeight);
                        }
                    }
                    else
                    {
                        var size = nodeItem.Size;
                        itemRect = new ImmutableRect_double(nodeItem.Pos.X, nodeItem.Pos.Y, size.Width, size.Height);
                    }
                }

                if (isNoViewportCulling || viewportRect.IntersectsWith(itemRect))
                {
                    if (nodePanel == null)
                    {
                        var style = nodeItem.InternalData().Style;

                        nodePanel = FindOrCreateNodePanel();

                        if (style == null)
                        {
                            if (nodeItemType == null)
                                nodeItemType = nodeItem.GetType();

                            if (_styleDict.TryGetValue(nodeItemType, out style) == false)
                            {
                                style = TryFindResource(nodeItemType) as Style;
                                _styleDict.Add(nodeItemType, style);
                            }

                            nodeItem.InternalData().Style = style;
                        }

                        if (nodePanel.Style == null ||
                            style == null ||
                            nodePanel.Style.GetHashCode() != style.GetHashCode())
                        {
                            if (nodePanel.DataContext != null)
                                nodePanel.DataContext = null;

                            nodePanel.Style = style;
                        }

                        nodePanel.DataContext = nodeItem;

                        if (NumberHelper.AreClose(nodePanel.Opacity, 1.0) == false)
                            nodePanel.Opacity = 1.0;

                        UpdateNodeSlotEnabled(nodeItem);

                        changedUpdate.Add((nodeItem, nodePanel));

                        AddChild(nodePanel);

                        nodePanel.InvalidateSlots();
                        nodePanel.InvalidateMeasure();
                    }
                }
                else
                {
                    if (nodePanel != null)
                    {
                        RemoveNodePanel(nodePanel);

                        changedUpdate.Add((nodeItem, null));
                    }
                }
            }

            foreach (var c in changedUpdate.Buffer)
                AddOrUpdate(c.Item1, c.Item2);
        }

        private BiaNodePanel FindOrCreateNodePanel()
        {
            BiaNodePanel panel;
            if (_recycleNodePanelPool.Count != 0)
            {
                panel = _recycleNodePanelPool.Pop();
            }
            else
            {
                panel = new BiaNodePanel();

                panel.MouseEnter += NodePanel_OnMouseEnter;
                panel.MouseLeave += NodePanel_OnMouseLeave;
                panel.MouseLeftButtonDown += NodePanel_OnMouseLeftButtonDown;
                panel.MouseMove += NodePanel_OnMouseMove;
            }

            return panel;
        }

        private void ClearSelectedNodes()
        {
            using var selectedNodes = _parent.NodeContainers.SelectMany(x => x.SelectedNodes).ToTempBuffer(256);

            foreach (var n in selectedNodes.Buffer)
                n.IsSelected = false;
        }

        private void UpdateNodeSlotEnabled(bool isStart)
        {
            var args = new BiaNodeSlotEnabledCheckerArgs(
                isStart
                    ? BiaNodeSlotEnableTiming.ConnectionStarting
                    : BiaNodeSlotEnableTiming.Default,
                new BiaNodeItemSlotIdPair(
                    _parent.SourceNodeSlotConnecting.Item,
                    _parent.SourceNodeSlotConnecting.Slot.Id
                ));

            foreach (var child in _parent.NodeContainers.SelectMany(x => x.Children))
            {
                if (child.IsActive == false)
                    continue;

                var nodeItem = (IBiaNodeItem) child.DataContext;

                UpdateNodeSlotEnabled(nodeItem, args);

                child.InvalidateSlots();

                if (isStart)
                    child.Opacity = nodeItem.InternalData().EnableSlots.Count > 0
                        ? 1.0
                        : 0.2;
                else
                    child.Opacity = 1.0;
            }
        }

        private void UpdateNodeSlotEnabled(IBiaNodeItem target, in BiaNodeSlotEnabledCheckerArgs args)
        {
            target.InternalData().EnableSlots.Clear();

            if (_parent.NodeSlotEnabledChecker == null)
            {
                var enabledSlotIds = target.Slots?.Keys;

                if (enabledSlotIds != null)
                {
                    foreach (var enabledSlotId in enabledSlotIds)
                    {
                        Debug.Assert(target.Slots != null);
                        Debug.Assert(target.Slots.ContainsKey(enabledSlotId));

                        var slot = target.Slots[enabledSlotId];
                        target.InternalData().EnableSlots.Add(slot);
                    }
                }
            }
            else
            {
                Span<int> buffer = stackalloc int[16];

                // ※引数に積むのため、using ぜずDispose()をコールする
                var enabledSlotIds = new TempBuffer<int>(buffer);

                try
                {
                    _parent.NodeSlotEnabledChecker.Check(target, args, ref enabledSlotIds);

                    var ids = enabledSlotIds.Buffer;
                    for (var i = 0; i != ids.Length; ++i)
                    {
                        Debug.Assert(target.Slots != null);
                        Debug.Assert(target.Slots.ContainsKey(ids[i]));

                        var slot = target.Slots[ids[i]];
                        target.InternalData().EnableSlots.Add(slot);
                    }
                }
                finally
                {
                    enabledSlotIds.Dispose();
                }
            }
        }

        private static readonly BiaNodeSlotEnabledCheckerArgs DefaultEnabledCheckerArgs =
            new BiaNodeSlotEnabledCheckerArgs(
                BiaNodeSlotEnableTiming.Default,
                new BiaNodeItemSlotIdPair(null, 0));

        private void UpdateNodeSlotEnabled(IBiaNodeItem target)
        {
            UpdateNodeSlotEnabled(target, DefaultEnabledCheckerArgs);
        }

        private void UpdateSelectedNode(IBiaNodeItem node)
        {
            if (node.Layer != _layer)
                return;

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
            INotifyCollectionChanged? oldSource,
            INotifyCollectionChanged? newSource)
        {
            if (oldSource != null)
            {
                // 全部削除として扱う
                NodesSourceOnCollectionChanged(null,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList) oldSource, 0));

                oldSource.CollectionChanged -= NodesSourceOnCollectionChanged;
            }

            if (newSource != null)
            {
                newSource.CollectionChanged += NodesSourceOnCollectionChanged;

                // 全部追加として扱う
                NodesSourceOnCollectionChanged(null,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList) newSource, 0));
            }
        }

        private void NodesSourceOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null)
                        break;

                    foreach (IBiaNodeItem? nodeItem in e.NewItems)
                    {
                        Debug.Assert(nodeItem != null);

                        if (nodeItem.Layer != _layer)
                            continue;

                        nodeItem.PropertyChanged += NodeItemPropertyChanged;

                        AddOrUpdate(nodeItem, null);
                        UpdateSelectedNode(nodeItem);
                    }

                    UpdateChildrenBag();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null)
                        break;

                    foreach (IBiaNodeItem? nodeItem in e.OldItems)
                    {
                        Debug.Assert(nodeItem != null);

                        if (nodeItem.Layer != _layer)
                            continue;

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
                    using var oldItems = e.OldItems.Cast<IBiaNodeItem>().ToTempBuffer(256);
                    using var newItems = e.NewItems.Cast<IBiaNodeItem>().ToTempBuffer(256);

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

                    UpdateChildrenBag();
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

            if (node.Layer != _layer)
                return;

            switch (e.PropertyName)
            {
                case nameof(IBiaNodeItem.Pos):
                case nameof(IBiaNodeItem.Size):
                {
                    _parent.InvokeNodeItemMoved();
                    ChangeElementInternal(node);
                    break;
                }

                case nameof(IBiaNodeItem.IsSelected):
                case nameof(IBiaNodeItem.IsPreSelected):
                case nameof(IBiaNodeItem.IsMouseOver):
                {
                    _parent.InvokeNodeItemMoved();
                    UpdateSelectedNode(node);
                    ChangeElementInternal(node);

                    if (e.PropertyName == nameof(IBiaNodeItem.IsSelected))
                    {
                        if (_nodeDict.TryGetValue(node, out var panel))
                        {
                            if (panel != null)
                                ToLast(panel);
                        }
                    }

                    break;
                }

                case nameof(IBiaNodeItem.Slots):
                {
                    ChangeElementInternal(node);

                    UpdateNodeSlotEnabled(node);

                    if (_nodeDict.TryGetValue(node, out var panel))
                    {
                        if (panel != null)
                        {
                            panel.InvalidateSlots();
                            _parent.InvokeLinkChanged();
                        }
                    }

                    break;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        private void ChangeElementInternal(IBiaNodeItem node)
        {
            var panel = FindPanel(node);
            if (panel == null)
                return;

            ChangeElement(panel);
            UpdateChildrenBag();
        }

        #region NodePanel

        private void NodePanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var panel = (BiaNodePanel) sender;

            Debug.Assert(panel.IsActive);
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            ToLast(panel);

            nodeItem.IsMouseOver = true;

            e.Handled = true;
        }

        private static void NodePanel_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var panel = (BiaNodePanel) sender;

            if (panel.IsActive == false)
                return;

            var nodeItem = (IBiaNodeItem) panel.DataContext;

            // 親コントロールがマウスキャプチャーしてもここに飛んでくる
            // IsMouseOverを作りたいので、マウス位置とノードコントロールの位置・サイズを見てマウスオーバー状態を作る
            var mouesPos = e.GetPosition(panel);
            if (mouesPos.X >= 0 && mouesPos.X < nodeItem.Size.Width &&
                mouesPos.Y >= 0 && mouesPos.Y < nodeItem.Size.Height)
                return;

            nodeItem.IsMouseOver = false;

            panel.InvalidateSlots();

            e.Handled = true;
        }

        private void NodePanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var panel = (BiaNodePanel) sender;

            Debug.Assert(panel.IsActive);
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            if (nodeItem.IsSelected == false)
            {
                // [Ctrl]押下で追加する
                if (KeyboardHelper.IsPressControl == false)
                {
                    _parent.InvokePropertyEditStarting();

                    ClearSelectedNodes();
                    nodeItem.IsSelected = true;

                    _parent.InvokePropertyEditCompleted();
                }
                else
                    nodeItem.IsSelected = true;
            }
            else
            {
                if (KeyboardHelper.IsPressControl)
                    nodeItem.IsSelected = false;
            }

            var pos = e.GetPosition(panel);
            var slot = nodeItem.FindSlotFromPos(Unsafe.As<Point, ImmutableVec2_double>(ref pos), this);

            // 元スロットが無効であればリンク処置は行わない
            if (slot != null)
            {
                var slotData = new BiaNodeItemSlotIdPair(nodeItem, slot.Id);

                if (_parent.NodeSlotEnabledChecker != null)
                {
                    if (_parent.NodeSlotEnabledChecker.IsEnableSlot(slotData) == false)
                        slot = null;
                }
            }

            _mouseOperator.OnMouseLeftButtonDown(
                e,
                slot == null
                    ? MouseOperator.TargetType.NodePanel
                    : MouseOperator.TargetType.NodeLink);

            if (_mouseOperator.IsLinkMove)
            {
                if (slot == null)
                    throw new NotSupportedException();

                if (_parent.CanConnectLink)
                {
                    var slotData = new BiaNodeItemSlotIdPair(nodeItem, slot.Id);
                    var args = new NodeLinkStartingEventArgs(slotData);
                    _parent.InvokeNodeLinkStarting(args);

                    _parent.SourceNodeSlotConnecting = new BiaNodeItemSlotPair(nodeItem, slot);
                    _parent.TargetNodeSlotConnecting = default;

                    UpdateNodeSlotEnabled(true);
                }
            }

            _mouseOperator.InvokePostMouseLeftButtonDown(e);

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
                if (child.IsActive == false)
                    continue;

                var pos = ((IBiaHasPos) child.DataContext).AlignPos();

                child.Arrange(new Rect(pos, child.DesiredSize));
            }
        }

        private static readonly Size MaxSize = new Size(double.MaxValue, double.MaxValue);

        protected override void MeasureChildren(IEnumerable<BiaNodePanel> children, Size availableSize)
        {
            foreach (var child in children)
            {
                if (child.IsActive == false)
                    continue;

                var nodeItem = (IBiaNodeItem) child.DataContext;

                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (nodeItem.Size.Width != 0)
                    child.Width = nodeItem.Size.Width;

                if (nodeItem.Size.Height != 0)
                    child.Height = nodeItem.Size.Height;
                // ReSharper restore CompareOfFloatsByEqualityOperator

                child.Measure(MaxSize);

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

        private readonly Dictionary<Type, Style?> _styleDict = new Dictionary<Type, Style?>();
        private readonly Dictionary<Type, Size> _panelDefaultSizeDict = new Dictionary<Type, Size>();
    }
}