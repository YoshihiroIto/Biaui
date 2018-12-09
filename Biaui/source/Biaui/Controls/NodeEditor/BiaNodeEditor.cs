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

        private readonly Dictionary<INodeItem, BiaNodePanel> _children = new Dictionary<INodeItem, BiaNodePanel>();
        private readonly TranslateTransform _translate = new TranslateTransform();
        private readonly ScaleTransform _scale = new ScaleTransform();
        private readonly ChildrenBag _childrenBag = new ChildrenBag();

        public BiaNodeEditor()
        {
            SizeChanged += (_, __) => MakeChildren();

            ClipToBounds = true;

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
                if (_children.TryGetValue(node, out var child))
                {
                    ChildrenBag.SetLocation(child, node.Pos);
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

                    foreach (INodeItem node in e.NewItems)
                    {
                        var child = new BiaNodePanel {DataContext = node};

                        ChildrenBag.SetLocation(child, node.Pos);

                        child.Width = 200;
                        child.Height = 300;

                        child.MouseEnter += (s, _) => SetFrontmost((BiaNodePanel) s);

                        _children.Add(node, child);
                    }

                    MakeChildren();

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
            _childrenBag.Children.Remove(child);
            _childrenBag.Children.Add(child);
        }

        private void MakeChildren()
        {
            MakeChildren(MakeCurrentViewport());

            _childrenBag.InvalidateMeasure();
        }

        private void MakeChildren(Rect rect)
        {
            foreach (var c in _children)
            {
                var m = c.Key;
                var t = c.Value;

                //if (m.IntersectsWith(t.Width, t.Height, rect))
                if (m.IntersectsWith(200, 300, rect))
                {
                    if (_childrenBag.Children.Contains(t) == false)
                        _childrenBag.Children.Add(t);
                }
                else
                {
                    if (_childrenBag.Children.Contains(t))
                        _childrenBag.Children.Remove(t);
                }
            }
        }

        #endregion

        #region Mouse

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var s = _scale.ScaleX;

            s *= e.Delta < 0 ? 1.25 : 1.0 / 1.25;

            var p = e.GetPosition(this);
            var d0 = ScenePosFromControlPos(p);

            s = Math.Max(Math.Min(s, 3.0), 0.25);
            _scale.ScaleX = s;
            _scale.ScaleY = s;

            var d1 = ScenePosFromControlPos(p);

            var diff = d1 - d0;

            _translate.X += diff.X * s;
            _translate.Y += diff.Y * s;

            MakeChildren();
        }

        private double _mouseDownScrollX;
        private double _mouseDownScrollY;
        private Point _mouseDownMousePos;
        private bool _isScrolling;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if ((Win32Helper.GetAsyncKeyState(Win32Helper.VK_SPACE) & 0x8000) == 0)
                return;

            _mouseDownScrollX = _translate.X;
            _mouseDownScrollY = _translate.Y;
            _mouseDownMousePos = e.GetPosition(this);

            _isScrolling = true;
            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _isScrolling = false;

            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isScrolling == false)
                return;

            var pos = e.GetPosition(this);
            var diff = pos - _mouseDownMousePos;

            _translate.X = _mouseDownScrollX + diff.X;
            _translate.Y = _mouseDownScrollY + diff.Y;

            MakeChildren();
        }

        private Point ScenePosFromControlPos(Point pos)
            => new Point(
                (pos.X - _translate.X) / _scale.ScaleX,
                (pos.Y - _translate.Y) / _scale.ScaleY);

        #endregion
    }

    internal class ChildrenBag : FrameworkElement
    {
        internal static Point GetLocation(DependencyObject obj)
        {
            return (Point) obj.GetValue(LocationProperty);
        }

        internal static void SetLocation(DependencyObject obj, Point value)
        {
            obj.SetValue(LocationProperty, value);
        }

        internal static readonly DependencyProperty LocationProperty =
            DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(ChildrenBag),
                new FrameworkPropertyMetadata(Boxes.Point00, FrameworkPropertyMetadataOptions.AffectsArrange));

        internal ObservableCollection<UIElement> Children { get; }

        internal ChildrenBag()
        {
            Children = new ObservableCollection<UIElement>();

            Children.CollectionChanged += Children_CollectionChanged;
        }

        private readonly List<UIElement> _changedElements = new List<UIElement>();

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (UIElement oldItem in e.OldItems)
                {
                    RemoveVisualChild(oldItem);
                }
            }

            if (e.NewItems != null)
            {
                foreach (UIElement newItem in e.NewItems)
                {
                    AddVisualChild(newItem);

                    _changedElements.Add(newItem);
                }
            }
        }

        protected override int VisualChildrenCount => Children.Count;

        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //foreach (var child in Children)
            foreach (var child in _changedElements)
            {
                var location = GetLocation(child);

                child.Arrange(new Rect(location, child.DesiredSize));
            }

            _changedElements.Clear();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //foreach (var child in Children)
            foreach (var child in _changedElements)
            {
                if (child is FrameworkElement fe)
                {
                    child.Measure(new Size(fe.Width, fe.Height));
                }
            }

            return base.MeasureOverride(availableSize);
        }
    }
}