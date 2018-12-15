using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Internals;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeEditor : Grid
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
                typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(ObservableCollection<INodeItem>),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;

                        var old = self._NodesSource;
                        self._NodesSource = (ObservableCollection<INodeItem>) e.NewValue;
                        self.UpdateNodesSource(old, self._NodesSource);
                    }));

        #endregion

        private readonly Dictionary<INodeItem, BiaNodePanel> _nodeDict = new Dictionary<INodeItem, BiaNodePanel>();
        private readonly ChildrenBag _childrenBag = new ChildrenBag();

        private readonly TranslateTransform _translate = new TranslateTransform();
        private readonly ScaleTransform _scale = new ScaleTransform();

        private readonly MouseOperator _mouseOperator;
        private readonly DispatcherTimer _removeNodePanelTimer;

        public BiaNodeEditor()
        {
            SizeChanged += (_, __) => UpdateChildrenBag(true);
            Unloaded += (_, __) => _removeNodePanelTimer.Stop();

            ClipToBounds = true;

            // ReSharper disable once VirtualMemberCallInConstructor
            Children.Add(new GridPanel(_translate, _scale));
            Children.Add(_childrenBag);

            var g = new TransformGroup();
            g.Children.Add(_scale);
            g.Children.Add(_translate);
            _childrenBag.RenderTransform = g;

            _mouseOperator = new MouseOperator(this, _translate, _scale);
            _mouseOperator.PanelMoving += OnPanelMoving;

            _removeNodePanelTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(100),
                DispatcherPriority.ApplicationIdle,
                (_, __) => DoRemoverNodePanel(),
                Dispatcher.CurrentDispatcher);

            _removeNodePanelTimer.Stop();
        }

        internal Point ScenePosFromControlPos(double x, double y)
            => new Point(
                (x - _translate.X) / _scale.ScaleX,
                (y - _translate.Y) / _scale.ScaleY);

        private ImmutableRect MakeCurrentViewport()
        {
            var sx = _scale.ScaleX;
            var sy = _scale.ScaleY;

            return new ImmutableRect(
                -_translate.X / sx,
                -_translate.Y / sy,
                ActualWidth / sx,
                ActualHeight / sy);
        }

        #region Children

        private void UpdateNodesSource(
            ObservableCollection<INodeItem> oldSource,
            ObservableCollection<INodeItem> newSource)
        {
            if (oldSource != null)
            {
                CleanAll();

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

        private void NodeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (INodeItem) sender;

            switch (e.PropertyName)
            {
                case nameof(INodeItem.Pos):
                case nameof(INodeItem.Size):
                {
                    ChangeElement(true);
                    break;
                }

                case nameof(INodeItem.IsSelected):
                {
                    if (node.IsSelected)
                        _selectedNodes.Add(node);
                    else
                        _selectedNodes.Remove(node);

                    ChangeElement(false);
                    break;
                }

                case nameof(INodeItem.IsPreSelected):
                {
                    if (node.IsPreSelected)
                        _preSelectedNodes.Add(node);
                    else
                        _preSelectedNodes.Remove(node);

                    ChangeElement(false);
                    break;
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////
            void ChangeElement(bool isPushRemove)
            {
                if (_nodeDict.TryGetValue(node, out var panel) == false)
                    return;

                if (panel != null)
                {
                    _childrenBag.ChangeElement(panel);

                    UpdateChildrenBag(isPushRemove);
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

                    foreach (INodeItem nodeItem in e.NewItems)
                    {
                        nodeItem.PropertyChanged += NodeItemPropertyChanged;
                        _nodeDict.Add(nodeItem, null);
                    }

                    UpdateChildrenBag(true);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null)
                        break;

                    foreach (INodeItem nodeItem in e.OldItems)
                    {
                        nodeItem.PropertyChanged -= NodeItemPropertyChanged;

                        if (_nodeDict.TryGetValue(nodeItem, out var panel))
                            RemoveNodePanel(panel);

                        _nodeDict.Remove(nodeItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    CleanAll();
                    break;

                case NotifyCollectionChangedAction.Replace:
                {
                    var oldItems = e.OldItems.Cast<INodeItem>().ToArray();
                    var newItems = e.NewItems.Cast<INodeItem>().ToArray();

                    if (oldItems.Length != newItems.Length)
                        throw new NotSupportedException();

                    for (var i = 0; i != oldItems.Length; ++i)
                    {
                        var oldItem = oldItems[i];
                        var newItem = newItems[i];

                        var panel = _nodeDict[oldItem];

                        oldItem.PropertyChanged -= NodeItemPropertyChanged;
                        _nodeDict.Remove(oldItem);

                        newItem.PropertyChanged += NodeItemPropertyChanged;
                        _nodeDict.Add(newItem, null);

                        if (panel != null)
                            RemoveNodePanel(panel);
                    }

                    UpdateChildrenBag(true);
                    break;
                }

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CleanAll()
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
        }

        private void SetFrontmost(BiaNodePanel child)
        {
            _childrenBag.ToLast(child);
        }

        private void UpdateChildrenBag(bool isPushRemove)
        {
            UpdateChildrenBag(MakeCurrentViewport(), isPushRemove);

            _childrenBag.InvalidateMeasure();
        }

        private int _isEnableUpdateChildrenBagDepth;

        private readonly List<(INodeItem, BiaNodePanel)> _changedUpdateChildrenBag =
            new List<(INodeItem, BiaNodePanel)>();

        public static readonly Size _maxSize = new Size(10000000, 10000000);

        private void UpdateChildrenBag(in ImmutableRect rect, bool isPushRemove)
        {
            if (_isEnableUpdateChildrenBagDepth > 0)
                return;

            // メモ：
            // 一見、以降のループ内でitem.SizeのSetterを呼び出し変更通知経由でメソッドに再入するように見えるが、
            // 対象のitemはまだ_childrenDictに登録されていないので問題ない(再入しない)。

            foreach (var c in _nodeDict)
            {
                var item = c.Key;
                var nodePanel = c.Value;

                var isTempSize = false;
                ImmutableRect itemRect;
                {
                    // 対象アイテムが一度も表示されていない場合は、大きさを適当に設定してしのぐ
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (item.Size.Width == 0 /* || item.Size.Height == 0*/)
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                    {
                        const double tempWidth = 256.0;
                        const double tempHeight = 512.0;

                        isTempSize = true;

                        var pos = item.Pos;
                        itemRect = new ImmutableRect(pos.X, pos.Y, pos.X + tempWidth, pos.Y + tempHeight);
                    }
                    else
                    {
                        var pos = item.Pos;
                        var size = item.Size;
                        itemRect = new ImmutableRect(pos.X, pos.Y, pos.X + size.Width, pos.Y + size.Height);
                    }
                }

                if (rect.IntersectsWith(itemRect))
                {
                    if (nodePanel == null)
                    {
                        nodePanel = GetNodePanel();
                        nodePanel.Style = FindResource(item.GetType()) as Style;

                        nodePanel.Measure(_maxSize);
                        item.Size = nodePanel.DesiredSize;

                        if (isTempSize == false || item.IntersectsWith(rect))
                        {
                            nodePanel.DataContext = item;

                            _changedUpdateChildrenBag.Add((item, nodePanel));
                            _childrenBag.AddChild(nodePanel);
                        }
                    }
                }
                else
                {
                    if (isPushRemove)
                    {
                        if (nodePanel != null)
                        {
                            RequestRemoveNodePanel(nodePanel);

                            _changedUpdateChildrenBag.Add((item, null));
                        }
                    }
                }
            }

            foreach (var c in _changedUpdateChildrenBag)
                _nodeDict[c.Item1] = c.Item2;

            _changedUpdateChildrenBag.Clear();
        }

        private void RemoveNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            ReturnNodePanel(panel);
            _childrenBag.RemoveChild(panel);
        }

        private void RequestRemoveNodePanel(BiaNodePanel panel)
        {
            _removeNodePanelPool.Push(panel);

            _removeNodePanelTimer.Start();
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

            //Debug.WriteLine($"DispatcherTimer>>>>>>>>>>{_removeNodePanelPool.Count}");
        }

        #endregion

        #region ノードパネル管理

        private readonly Stack<BiaNodePanel> _recycleNodePanelPool = new Stack<BiaNodePanel>();
        private readonly Stack<BiaNodePanel> _removeNodePanelPool = new Stack<BiaNodePanel>();

        private readonly HashSet<INodeItem> _selectedNodes = new HashSet<INodeItem>();
        private readonly HashSet<INodeItem> _preSelectedNodes = new HashSet<INodeItem>();

        private BiaNodePanel GetNodePanel()
        {
            if (_recycleNodePanelPool.Count != 0)
                return _recycleNodePanelPool.Pop();

            var p = new BiaNodePanel();

            p.MouseEnter += NodePanel_OnMouseEnter;
            p.MouseLeftButtonDown += NodePanel_OnMouseLeftButtonDown;
            p.MouseLeftButtonUp += NodePanel_OnMouseLeftButtonUp;
            p.MouseMove += NodePanel_OnMouseMove;

            return p;
        }

        private void ReturnNodePanel(BiaNodePanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _recycleNodePanelPool.Push(panel);
        }

        private void NodePanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SetFrontmost((BiaNodePanel) sender);

            e.Handled = true;
        }

        private void NodePanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var p = (BiaNodePanel) sender;
            var i = (INodeItem) p.DataContext;

            if (i.IsSelected == false)
            {
                var isPressControl = KeyboardHelper.IsPressControl;

                // [Ctrl]押下で追加する
                if (isPressControl == false)
                {
                    ClearSelectedNode();

                    i.IsSelected = true;
                }
                else
                    i.IsSelected = !i.IsSelected;
            }

            _mouseOperator.OnMouseLeftButtonDown(e, MouseOperator.TargetType.NodePanel);

            e.Handled = true;
        }

        private void NodePanel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseOperator.OnMouseLeftButtonUp(e);

            e.Handled = true;
        }

        private void NodePanel_OnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseOperator.OnMouseMove(e);

            e.Handled = true;
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

        private void ClearSelectedNode()
        {
            foreach (var n in _selectedNodes.ToArray())
                n.IsSelected = false;
        }

        private void ClearPreSelectedNode()
        {
            foreach (var n in _preSelectedNodes.ToArray())
                n.IsPreSelected = false;
        }

        #endregion

        #region Mouse

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _mouseOperator.OnMouseWheel(e);

            UpdateChildrenBag(true);

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _mouseOperator.OnMouseLeftButtonDown(e, MouseOperator.TargetType.NodeEditor);

            if (_mouseOperator.IsBoxSelect)
                AddBoxSelector();

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_mouseOperator.IsBoxSelect)
            {
                SelectNodes(_boxSelector.Rect);
                ClearPreSelectedNode();

                RemoveBoxSelector();
            }

            if (_mouseOperator.IsPanelMove)
            {
                ++_isEnableUpdateChildrenBagDepth;
                {
                    foreach (var n in _selectedNodes)
                    {
                        n.Pos = new Point(
                            Math.Round(n.Pos.X / 32) * 32,
                            Math.Round(n.Pos.Y / 32) * 32);
                    }
                }
                --_isEnableUpdateChildrenBagDepth;
                Debug.Assert(_isEnableUpdateChildrenBagDepth >= 0);
            }

            UpdateChildrenBag(true);

            _mouseOperator.OnMouseLeftButtonUp(e);

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _mouseOperator.OnMouseMove(e);

            if (_mouseOperator.IsOperating)
                UpdateChildrenBag(false);

            if (_mouseOperator.IsBoxSelect)
            {
                UpdateBoxSelector();
                PreSelectNodes(_boxSelector.Rect);
            }

            e.Handled = true;
        }

        private void SelectNodes(in ImmutableRect rect)
        {
            var isPressControl = KeyboardHelper.IsPressControl;

            // [Ctrl]押下で追加する
            if (isPressControl == false)
                ClearSelectedNode();

            if (NodesSource == null)
                return;

            foreach (var node in NodesSource)
            {
                var nr = new ImmutableRect(node.Pos, node.Size);

                if (rect.IntersectsWith(nr))
                {
                    if (isPressControl)
                        node.IsSelected = !node.IsSelected;
                    else
                        node.IsSelected = true;
                }
            }
        }

        private void PreSelectNodes(in ImmutableRect rect)
        {
            ClearPreSelectedNode();

            foreach (var node in NodesSource)
            {
                var nr = new ImmutableRect(node.Pos, node.Size);

                if (rect.IntersectsWith(nr))
                    node.IsPreSelected = true;
            }
        }

        #endregion

        #region BoxSelect

        private readonly BoxSelector _boxSelector = new BoxSelector();

        private void AddBoxSelector()
        {
            _boxSelector.Rect = new ImmutableRect(0, 0, 0, 0);

            _childrenBag.AddChild(_boxSelector);
            UpdateChildrenBag(true);
        }

        private void RemoveBoxSelector()
        {
            _childrenBag.RemoveChild(_boxSelector);
            UpdateChildrenBag(true);
        }

        private void UpdateBoxSelector()
        {
            var s = _scale.ScaleX;
            var tx = _translate.X;
            var ty = _translate.Y;

            var sr = _mouseOperator.SelectionRect;

            var p0 = (Point) ((sr.P0 - new Point(tx, ty)) / s);
            var p1 = (Point) ((sr.P1 - new Point(tx, ty)) / s);

            _boxSelector.Rect = new ImmutableRect(p0, new Size(p1.X - p0.X, p1.Y - p0.Y));
            _childrenBag.ChangeElement(_boxSelector);
            UpdateChildrenBag(true);
        }

        #endregion
    }
}