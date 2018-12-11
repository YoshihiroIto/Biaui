using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeEditor : Border
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

        private ObservableCollection<INodeItem> _NodesSource = default(ObservableCollection<INodeItem>);

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

        private readonly Dictionary<INodeItem, BiaNodePanel> _childrenDict = new Dictionary<INodeItem, BiaNodePanel>();
        private readonly ChildrenBag _childrenBag = new ChildrenBag();

        private readonly TranslateTransform _translate = new TranslateTransform();
        private readonly ScaleTransform _scale = new ScaleTransform();

        private readonly MouseOperator _mouseOperator;

        public BiaNodeEditor()
        {
            SizeChanged += (_, __) => UpdateChildrenBag();

            ClipToBounds = true;

            // ReSharper disable once VirtualMemberCallInConstructor
            Child = _childrenBag;

            var g = new TransformGroup();
            g.Children.Add(_scale);
            g.Children.Add(_translate);
            _childrenBag.RenderTransform = g;

            _mouseOperator = new MouseOperator(this, _translate, _scale);
            _mouseOperator.PanelMoving += OnPanelMoving;
        }

        internal Point ScenePosFromControlPos(double x, double y)
            => new Point(
                (x - _translate.X) / _scale.ScaleX,
                (y - _translate.Y) / _scale.ScaleY);

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _scale.ScaleX;
            var tx = _translate.X;
            var ty = _translate.Y;

            var bx = FrameworkElementHelper.RoundLayoutValue(ActualWidth);
            var by = FrameworkElementHelper.RoundLayoutValue(ActualHeight);

            for (var h = 0;; ++h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x < 0) continue;
                if (x > ActualWidth) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var h = 0;; --h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x > ActualWidth) continue;
                if (x < 0) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var v = 0;; ++v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y < 0) continue;
                if (y > ActualHeight) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }


            for (var v = 0;; --v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y > ActualHeight) continue;
                if (y < 0) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }
        }

        private Rect MakeCurrentViewport()
        {
            var sx = _scale.ScaleX;
            var sy = _scale.ScaleY;

            return new Rect(
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
                foreach (var i in oldSource)
                    i.PropertyChanged -= NodeItemPropertyChanged;

                oldSource.CollectionChanged -= NodesSourceOnCollectionChanged;
            }

            if (newSource != null)
            {
                foreach (var i in newSource)
                    i.PropertyChanged += NodeItemPropertyChanged;

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
                {
                    if (_childrenDict.TryGetValue(node, out var child))
                        if (child != null)
                        {
                            ChildrenBag.SetPos(child, node.Pos);
                            _childrenBag.ChangeElement(child);
                        }

                    break;
                }

                case nameof(INodeItem.Size):
                {
                    if (_childrenDict.TryGetValue(node, out var child))
                        if (child != null)
                        {
                            ChildrenBag.SetSize(child, node.Size);
                            _childrenBag.ChangeElement(child);
                        }

                    break;
                }

                case nameof(INodeItem.IsSelected):
                {
                    if (node.IsSelected)
                        _selectedNodes.Add(node);
                    else
                        _selectedNodes.Remove(node);

                    if (_childrenDict.TryGetValue(node, out var child))
                        if (child != null)
                            _childrenBag.ChangeElement(child);

                    break;
                }

                case nameof(INodeItem.IsPreSelected):
                {
                    if (node.IsPreSelected)
                        _preSelectedNodes.Add(node);
                    else
                        _preSelectedNodes.Remove(node);

                    if (_childrenDict.TryGetValue(node, out var child))
                        if (child != null)
                            _childrenBag.ChangeElement(child);

                    break;
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
                        _childrenDict.Add(nodeItem, null);

                    UpdateChildrenBag();

                    break;

                case NotifyCollectionChangedAction.Remove:
                    throw new NotImplementedException();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetFrontmost(BiaNodePanel child)
        {
            _childrenBag.ToLast(child);
        }

        private void UpdateChildrenBag()
        {
            UpdateChildrenBag(MakeCurrentViewport());

            _childrenBag.InvalidateMeasure();
        }

        private readonly List<(INodeItem, BiaNodePanel)> _changedUpdateChildrenBag =
            new List<(INodeItem, BiaNodePanel)>();

        private void UpdateChildrenBag(Rect rect)
        {
            foreach (var c in _childrenDict)
            {
                var m = c.Key;
                var t = c.Value;

                if (m.IntersectsWith(rect))
                {
                    if (t == null)
                    {
                        t = GetNodePanel();

                        t.DataContext = m;
                        ChildrenBag.SetPos(t, m.Pos);
                        ChildrenBag.SetSize(t, m.Size);

                        t.Width = m.Size.Width;
                        t.Height = m.Size.Height;

                        _changedUpdateChildrenBag.Add((m, t));
                    }

                    _childrenBag.AddChild(t);
                }
                else
                {
                    if (t != null)
                    {
                        ReturnNodePanel(t);

                        _childrenBag.RemoveChild(t);

                        _changedUpdateChildrenBag.Add((m, null));
                    }
                }
            }

            foreach (var c in _changedUpdateChildrenBag)
                _childrenDict[c.Item1] = c.Item2;

            _changedUpdateChildrenBag.Clear();
        }

        #endregion

        #region ノードパネル管理

        private readonly Stack<BiaNodePanel> _nodePanelPool = new Stack<BiaNodePanel>();

        private readonly HashSet<INodeItem> _selectedNodes = new HashSet<INodeItem>();
        private readonly HashSet<INodeItem> _preSelectedNodes = new HashSet<INodeItem>();

        private BiaNodePanel GetNodePanel()
        {
            if (_nodePanelPool.Count != 0)
                return _nodePanelPool.Pop();

            var p = new BiaNodePanel();

            p.MouseEnter += NodePanel_OnMouseEnter;
            p.MouseLeftButtonDown += NodePanel_OnMouseLeftButtonDown;
            p.MouseLeftButtonUp += NodePanel_OnMouseLeftButtonUp;
            p.MouseMove += NodePanel_OnMouseMove;

            return p;
        }

        private void ReturnNodePanel(BiaNodePanel p)
        {
            _nodePanelPool.Push(p);
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
            foreach (var n in _selectedNodes)
                n.Pos += e.Diff;
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

            UpdateChildrenBag();

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
                foreach (var n in _selectedNodes)
                {
                    n.Pos = new Point(
                        Math.Round(n.Pos.X / 32) * 32,
                        Math.Round(n.Pos.Y / 32) * 32);
                }

                UpdateChildrenBag();
            }

            _mouseOperator.OnMouseLeftButtonUp(e);

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _mouseOperator.OnMouseMove(e);

            if (_mouseOperator.IsOperating)
                UpdateChildrenBag();

            if (_mouseOperator.IsBoxSelect)
            {
                UpdateBoxSelector();
                PreSelectNodes(_boxSelector.Rect);
            }

            e.Handled = true;
        }

        private void SelectNodes(Rect rect)
        {
            var isPressControl = KeyboardHelper.IsPressControl;

            // [Ctrl]押下で追加する
            if (isPressControl == false)
                ClearSelectedNode();

            if (NodesSource == null)
                return;

            foreach (var node in NodesSource)
            {
                var nr = new Rect(node.Pos, node.Size);

                if (rect.IntersectsWith(nr))
                {
                    if (isPressControl)
                        node.IsSelected = !node.IsSelected;
                    else
                        node.IsSelected = true;
                }
            }
        }

        private void PreSelectNodes(Rect rect)
        {
            ClearPreSelectedNode();

            foreach (var node in NodesSource)
            {
                var nr = new Rect(node.Pos, node.Size);

                if (rect.IntersectsWith(nr))
                    node.IsPreSelected = true;
            }
        }

        #endregion

        #region BoxSelect

        private readonly BoxSelector _boxSelector = new BoxSelector();

        private void AddBoxSelector()
        {
            _boxSelector.Rect = Rect.Empty;

            _childrenBag.AddChild(_boxSelector);
            UpdateChildrenBag();
        }

        private void RemoveBoxSelector()
        {
            _childrenBag.RemoveChild(_boxSelector);
            UpdateChildrenBag();
        }

        private void UpdateBoxSelector()
        {
            var s = _scale.ScaleX;
            var tx = _translate.X;
            var ty = _translate.Y;

            var sr = _mouseOperator.SelectionRect;

            var p0 = (Point) ((sr.P0 - new Point(tx, ty)) / s);
            var p1 = (Point) ((sr.P1 - new Point(tx, ty)) / s);

            _boxSelector.Rect = new Rect(p0, p1);
            _childrenBag.ChangeElement(_boxSelector);
            UpdateChildrenBag();
        }

        #endregion
    }

    internal class ChildrenBag : FrameworkElement
    {
        #region Pos

        internal static Point GetPos(DependencyObject obj)
        {
            return (Point) obj.GetValue(PosProperty);
        }

        internal static void SetPos(DependencyObject obj, Point value)
        {
            obj.SetValue(PosProperty, value);
        }

        internal static readonly DependencyProperty PosProperty =
            DependencyProperty.RegisterAttached("Pos", typeof(Point), typeof(ChildrenBag),
                new FrameworkPropertyMetadata(Boxes.Point00, FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion

        #region Size

        internal static Size GetSize(DependencyObject obj)
        {
            return (Size) obj.GetValue(SizeProperty);
        }

        internal static void SetSize(DependencyObject obj, Size value)
        {
            obj.SetValue(SizeProperty, value);
        }

        internal static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(Size), typeof(ChildrenBag),
                new FrameworkPropertyMetadata(Boxes.Size11, FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion

        private readonly List<UIElement> _children = new List<UIElement>();
        private readonly HashSet<UIElement> _childrenForSearch = new HashSet<UIElement>();

        private readonly List<UIElement> _changedElements = new List<UIElement>();

        internal void AddChild(UIElement child)
        {
            if (_childrenForSearch.Contains(child))
                return;

            _children.Add(child);
            _childrenForSearch.Add(child);

            AddVisualChild(child);
            ChangeElement(child);
        }

        internal void RemoveChild(UIElement child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            _childrenForSearch.Remove(child);

            RemoveVisualChild(child);
        }

        internal void ToLast(UIElement child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            RemoveVisualChild(child);

            _children.Add(child);
            AddVisualChild(child);
        }

        internal void ChangeElement(UIElement child)
        {
            _changedElements.Add(child);
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in _changedElements)
            {
                var pos = GetPos(child);

                child.Arrange(new Rect(pos, child.DesiredSize));
            }

            _changedElements.Clear();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in _changedElements)
            {
                var size = GetSize(child);

                child.Measure(size);
            }

            return base.MeasureOverride(availableSize);
        }
    }
}