using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class NodeContainer
    {
        internal IEnumerable<KeyValuePair<IBiaNodeItem, BiaNodePanel>> Items => _nodeDict;

        private readonly BiaNodeEditor _parent;
        private readonly MouseOperator _mouseOperator;

        private readonly Dictionary<IBiaNodeItem, BiaNodePanel> _nodeDict
            = new Dictionary<IBiaNodeItem, BiaNodePanel>();

        private readonly List<(IBiaNodeItem, BiaNodePanel)> _changedUpdate = new List<(IBiaNodeItem, BiaNodePanel)>();

        private readonly Stack<BiaNodePanel> _removeNodePanelPool = new Stack<BiaNodePanel>();
        private readonly Stack<BiaNodePanel> _recycleNodePanelPool = new Stack<BiaNodePanel>();
        private readonly HashSet<IBiaNodeItem> _selectedNodes = new HashSet<IBiaNodeItem>();
        private readonly HashSet<IBiaNodeItem> _preSelectedNodes = new HashSet<IBiaNodeItem>();

        private readonly DispatcherTimer _removeNodePanelTimer;

        private int _isEnableUpdateChildrenBagDepth;

        internal NodeContainer(BiaNodeEditor parent, MouseOperator mouseOperator)
        {
            _parent = parent;
            _mouseOperator = mouseOperator;

            _mouseOperator.PanelMoving += OnPanelMoving;
            _mouseOperator.MouseWheel += OnMouseWheel;
            _mouseOperator.MouseMove += OnMouseMove;
            _mouseOperator.MouseLeftButtonUp += MouseOperatorOnMouseLeftButtonUp;

            _removeNodePanelTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(1000),
                DispatcherPriority.ApplicationIdle,
                (_, __) => DoRemoverNodePanel(),
                Dispatcher.CurrentDispatcher);

            _removeNodePanelTimer.Stop();
        }

        private void MouseOperatorOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                if (_parent.TargetNodePortConnecting != null)
                {
                    _parent.InvokeNodeLinkCompleted(
                        new NodeLinkCompletedEventArgs(
                            _parent.SourceNodePortConnecting.ToItemPortIdPair(),
                            _parent.TargetNodePortConnecting.ToItemPortIdPair()));
                }

                UpdateNodePortEnabled(false);
            }

            _parent.SourceNodePortConnecting = null;
            _parent.TargetNodePortConnecting = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseOperator.IsOperating)
                UpdateChildrenBag(false);

            if (_mouseOperator.IsBoxSelect)
                PreSelectNodes(_parent.TransformRect(_mouseOperator.SelectionRect));
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            UpdateChildrenBag(true);
        }

        internal void Stop()
        {
            _removeNodePanelTimer.Stop();
        }

        internal void AddOrUpdate(IBiaNodeItem nodeItem, BiaNodePanel panel)
        {
            _nodeDict[nodeItem] = panel;
        }

        internal void Remove(IBiaNodeItem nodeItem)
        {
            _nodeDict.Remove(nodeItem);
            _selectedNodes.Remove(nodeItem);
            _preSelectedNodes.Remove(nodeItem);
        }

        internal void Clear()
        {
            _nodeDict.Clear();

            _selectedNodes.Clear();
            _preSelectedNodes.Clear();
        }

        internal void ClearPreSelectedNode()
        {
            foreach (var n in _preSelectedNodes.ToArray())
                n.IsPreSelected = false;
        }

        internal void CleanAll()
        {
            foreach (var i in Items)
            {
                var nodeItem = i.Key;
                var panel = i.Value;

                nodeItem.PropertyChanged -= _parent.NodeItemPropertyChanged;

                if (panel != null)
                    RemoveNodePanel(panel);
            }

            Clear();

            _parent._backgroundPanel.InvalidateVisual();
        }

        internal void RemoveNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            ReturnNodePanel(panel);
            _parent._nodePanelBag.RemoveChild(panel);
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

        internal void SelectNodes(in ImmutableRect rect)
        {
            ++_isEnableUpdateChildrenBagDepth;
            {
                // [Ctrl]押下で追加する
                if (KeyboardHelper.IsPressControl == false)
                    ClearSelectedNode();

                foreach (var c in _parent._nodePanelBag.Children)
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

        internal void PreSelectNodes(in ImmutableRect rect)
        {
            ++_isEnableUpdateChildrenBagDepth;
            {
                ClearPreSelectedNode();

                foreach (var c in _parent._nodePanelBag.Children)
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

        private static readonly RectangleGeometry _rectGeom = new RectangleGeometry();

        private static bool IsHitVisual(in ImmutableRect rect, IBiaNodeItem node, Visual panel)
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


        internal void AlignSelectedNodes()
        {
            ++_isEnableUpdateChildrenBagDepth;

            foreach (var n in _selectedNodes)
            {
                n.Pos = new Point(
                    Math.Round(n.Pos.X / 32) * 32,
                    Math.Round(n.Pos.Y / 32) * 32);
            }

            --_isEnableUpdateChildrenBagDepth;
            Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
        }

        private void ReturnNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _recycleNodePanelPool.Push(panel);
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


        internal BiaNodePanel FindPanel(IBiaNodeItem nodeItem)
        {
            return _nodeDict.TryGetValue(nodeItem, out var panel) == false
                ? null
                : panel;
        }

        internal void UpdateChildrenBag(bool isPushRemove)
        {
            if (_isEnableUpdateChildrenBagDepth > 0)
                return;

            var viewPortRect = _parent.TransformRect(_parent.ActualWidth, _parent.ActualHeight);

            UpdateChildrenBag(viewPortRect, isPushRemove);

            _parent._nodePanelBag.InvalidateMeasure();
        }

        internal void UpdateChildrenBag(in ImmutableRect viewportRect, bool isPushRemove)
        {
            // メモ：
            // 一見、以降のループ内でitem.SizeのSetterを呼び出し変更通知経由でメソッドに再入するように見えるが、
            // 対象のitemはまだ_nodeDictに登録されていないので問題ない(再入しない)。

            foreach (var c in Items)
            {
                var item = c.Key;
                var nodePanel = c.Value;

                ImmutableRect itemRect;
                {
                    // 対象アイテムが一度も表示されていない場合は、大きさを適当に設定してしのぐ
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (item.Size.Width == 0 /* || item.Size.Height == 0*/)
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                    {
                        const double tempWidth = 256.0;
                        const double tempHeight = 512.0;

                        var pos = item.Pos;
                        itemRect = new ImmutableRect(pos.X, pos.Y, tempWidth, tempHeight);
                    }
                    else
                    {
                        var pos = item.Pos;
                        var size = item.Size;
                        itemRect = new ImmutableRect(pos.X, pos.Y, size.Width, size.Height);
                    }
                }

                if (viewportRect.IntersectsWith(itemRect))
                {
                    if (nodePanel == null)
                    {
                        bool isAdded;
                        (nodePanel, isAdded) = FindOrCreateNodePanel();

                        nodePanel.Style = _parent.FindResource(item.GetType()) as Style;
                        nodePanel.DataContext = item;
                        nodePanel.Opacity = 1.0;

                        _changedUpdate.Add((item, nodePanel));

                        if (isAdded)
                            _parent._nodePanelBag.ChangeElement(nodePanel);
                        else
                            _parent._nodePanelBag.AddChild(nodePanel);
                    }
                }
                else
                {
                    if (isPushRemove)
                    {
                        if (nodePanel != null)
                        {
                            RequestRemoveNodePanel(nodePanel);

                            _changedUpdate.Add((item, null));
                        }
                    }
                }
            }

            foreach (var c in _changedUpdate)
                AddOrUpdate(c.Item1, c.Item2);

            _changedUpdate.Clear();
        }

        private void RequestRemoveNodePanel(BiaNodePanel panel)
        {
            _removeNodePanelPool.Push(panel);

            _removeNodePanelTimer.Start();
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

        private void SetFrontmost(BiaNodePanel child)
        {
            _parent._nodePanelBag.ToLast(child);
        }

        private void ClearSelectedNode()
        {
            foreach (var n in _selectedNodes.ToArray())
                n.IsSelected = false;
        }

        internal void UpdateNodePortEnabled(bool isStart)
        {
            if (_parent.NodePortEnabledChecker == null)
                return;

            var args = new BiaNodePortEnabledCheckerArgs(
                isStart
                    ? BiaNodePortEnableTiming.ConnectionStarting
                    : BiaNodePortEnableTiming.Default,
                _parent.SourceNodePortConnecting.Item,
                _parent.SourceNodePortConnecting.Port.Id);

            foreach (var child in _parent._nodePanelBag.Children)
            {
                var nodeItem = (IBiaNodeItem) child.DataContext;

                UpdateNodePortEnabled(nodeItem, args);

                child.InvalidatePorts();

                InternalBiaNodeItemData internalData;

                if (nodeItem.InternalData == null)
                {
                    internalData = new InternalBiaNodeItemData();
                    nodeItem.InternalData = internalData;
                }
                else
                {
                    internalData = (InternalBiaNodeItemData) nodeItem.InternalData;
                }

                if (isStart)
                    child.Opacity = internalData.EnablePorts.Count > 0
                        ? 1.0
                        : 0.2;
                else
                    child.Opacity = 1.0;
            }
        }

        private void UpdateNodePortEnabled(IBiaNodeItem target, in BiaNodePortEnabledCheckerArgs args)
        {
            InternalBiaNodeItemData internalData;

            if (target.InternalData == null)
            {
                internalData = new InternalBiaNodeItemData();
                target.InternalData = internalData;
            }
            else
            {
                internalData = (InternalBiaNodeItemData) target.InternalData;
            }

            internalData.EnablePorts.Clear();

            foreach (var enabledPortId in _parent.NodePortEnabledChecker.Check(target, args))
            {
                Debug.Assert(target.Layout.Ports.ContainsKey(enabledPortId));

                var port = target.Layout.Ports[enabledPortId];
                internalData.EnablePorts.Add(port);
            }
        }

        internal void UpdateSelectedNode(IBiaNodeItem node)
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

        private void NodePanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var panel = (BiaNodePanel) sender;
            var nodeItem = (IBiaNodeItem) panel.DataContext;

            SetFrontmost(panel);

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
                _parent.TargetNodePortConnecting = null;

                UpdateNodePortEnabled(true);
            }

            _parent._backgroundPanel.InvalidateVisual();

            e.Handled = true;
        }

        private void NodePanel_OnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseOperator.OnMouseMove(e);

            //e.Handled = true;
        }
    }
}