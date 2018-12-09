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

        #region BorderColor

        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private Color _BorderColor = Colors.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaNodeEditor),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._BorderColor = (Color) e.NewValue;
                    }));

        #endregion

        private readonly Dictionary<INodeItem, BiaNodePanel> _children = new Dictionary<INodeItem, BiaNodePanel>();

        private readonly LazyRunner _CullingChildrenRunner;
        private readonly TranslateTransform _translate = new TranslateTransform();
        private readonly ScaleTransform _scale = new ScaleTransform();

        private readonly Canvas _childrenBag;

        public BiaNodeEditor()
        {
            _CullingChildrenRunner = new LazyRunner(CullingChildren);
            Unloaded += (_, __) => _CullingChildrenRunner.Dispose();

            SizeChanged += (_, __) => MakeChildren();
            ClipToBounds = true;

            _childrenBag = new Canvas();

            // ReSharper disable once VirtualMemberCallInConstructor
            Child = _childrenBag;

            var g = new TransformGroup();
            g.Children.Add(_scale);
            g.Children.Add(_translate);
            _childrenBag.RenderTransform = g;
        }

        private Rect MakeCurrentViewport() =>
            new Rect(
                -_translate.X / _scale.ScaleX,
                -_translate.Y / _scale.ScaleY,
                ActualWidth / _scale.ScaleX,
                ActualHeight / _scale.ScaleY);

        #region Children

        private void UpdateNodesSource(ObservableCollection<INodeItem> oldSource,
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
                    Canvas.SetLeft(child, node.Pos.X);
                    Canvas.SetTop(child, node.Pos.Y);
                }
            }
        }

        private void NodesSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var oldItems = e.OldItems?.Cast<INodeItem>();
            var newItems = e.NewItems?.Cast<INodeItem>();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (newItems != null)
                    {
                        //var viewport = MakeCurrentViewport();

                        foreach (var node in newItems)
                        {
                            var child = new BiaNodePanel {DataContext = node};

                            var a = child.ApplyTemplate();

                            Canvas.SetLeft(child, node.Pos.X);
                            Canvas.SetTop(child, node.Pos.Y);
                            child.Width = 200;
                            child.Height = 300;

                            child.MouseEnter += (s, _) => SetFrontmost((BiaNodePanel) s);

                            _children.Add(node, child);

                            //var childRect = new Rect(node.Pos.X, node.Pos.Y, child.Width, child.Height);
                            //if (viewport.IntersectsWith(childRect))
                            //    _childrenBag.Children.Add(child);
                        }
                    }

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

            _CullingChildrenRunner.Run();
        }

        private void MakeChildren(Rect rect)
        {
            foreach (var c in _children)
            {
                var m = c.Key;
                var t = c.Value;

                if (m.IntersectsWith(t.Width, t.Height, rect))
                    if (_childrenBag.Children.Contains(t) == false)
                        _childrenBag.Children.Add(t);
            }
        }

        private void CullingChildren()
        {
            var rect = MakeCurrentViewport();

            foreach (var c in _children)
            {
                var m = c.Key;
                var t = c.Value;

                if (m.IntersectsWith(t.Width, t.Height, rect) == false)
                    if (_childrenBag.Children.Contains(t))
                        _childrenBag.Children.Remove(t);
            }

            Debug.WriteLine($"CullingChildren() -- count:{_childrenBag.Children.Count}");
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

            _mouseDownScrollX = _translate.X;
            _mouseDownScrollY = _translate.Y;
            _mouseDownMousePos = e.GetPosition(this);

            if ((Win32Helper.GetAsyncKeyState(Win32Helper.VK_SPACE) & 0x8000) == 0)
                return;

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

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

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
}