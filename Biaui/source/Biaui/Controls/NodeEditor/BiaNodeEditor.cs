using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

            _mouseOperator = new MouseOperator(this, _translate, _scale);

            // ReSharper disable once VirtualMemberCallInConstructor
            Child = _childrenBag;

            var g = new TransformGroup();
            g.Children.Add(_scale);
            g.Children.Add(_translate);
            _childrenBag.RenderTransform = g;
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

            if (e.PropertyName == nameof(INodeItem.Pos))
            {
                if (_childrenDict.TryGetValue(node, out var child))
                {
                    ChildrenBag.SetPos(child, node.Pos);
                    ChildrenBag.SetSize(child, node.Size);
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

        private readonly List<(INodeItem, BiaNodePanel)> _changedUpdateChildrenBag = new List<(INodeItem, BiaNodePanel)>();

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

        public readonly Stack<BiaNodePanel> _NodePanelPool = new Stack<BiaNodePanel>();

        private BiaNodePanel GetNodePanel()
        {
            if (_NodePanelPool.Count != 0)
                return _NodePanelPool.Pop();

            var p = new BiaNodePanel();

            p.MouseEnter += NodePanel_OnMouseEnter;
            p.MouseLeftButtonDown += NodePanel_OnMouseLeftButtonDown;
                
            return p;
        }

        private void ReturnNodePanel(BiaNodePanel p)
        {
            _NodePanelPool.Push(p);
        }

        private void NodePanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void NodePanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SetFrontmost((BiaNodePanel)sender);
        }

        #endregion

        #region Mouse

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _mouseOperator.OnMouseWheel(e);

            UpdateChildrenBag();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _mouseOperator.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _mouseOperator.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _mouseOperator.OnMouseMove(e);

            if (_mouseOperator.IsScrolling)
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
            _changedElements.Add(child);
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

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //foreach (var child in _children)
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
            //foreach (var child in _children)
            foreach (var child in _changedElements)
            {
                var size = GetSize(child);

                child.Measure(size);
            }

            return base.MeasureOverride(availableSize);
        }
    }
}