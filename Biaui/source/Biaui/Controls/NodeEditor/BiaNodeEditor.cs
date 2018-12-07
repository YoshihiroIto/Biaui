using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodeEditor : Canvas
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

        #region ScrollViewer

        public ScrollViewer ScrollViewer
        {
            get => _ScrollViewer;
            set
            {
                if (value != _ScrollViewer)
                    SetValue(ScrollViewerProperty, value);
            }
        }

        private ScrollViewer _ScrollViewer = default(ScrollViewer);

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(BiaNodeEditor),
                new PropertyMetadata(
                    default(ScrollViewer),
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;

                        var old = self._ScrollViewer;
                        self._ScrollViewer = (ScrollViewer) e.NewValue;

                        self.UpdateScrollViewer(old, self._ScrollViewer);
                    }));

        #endregion

        #region Scale

        public double Scale
        {
            get => _Scale;
            set
            {
                if (NumberHelper.AreClose(value, _Scale) == false)
                    SetValue(ScaleProperty, value);
            }
        }

        private double _Scale = 1;

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(BiaNodeEditor),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaNodeEditor) s;
                        self._Scale = (double) e.NewValue;
                        self.MakeChildren();
                    }));

        #endregion

        private readonly Dictionary<INodeItem, BiaNodePanel> _children = new Dictionary<INodeItem, BiaNodePanel>();

        private readonly LazyRunner _CullingChildrenRunner;

        public BiaNodeEditor()
        {
            _CullingChildrenRunner = new LazyRunner(CullingChildren);
            Unloaded += (_, __) => _CullingChildrenRunner.Dispose();
        }

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
                    SetLeft(child, node.Pos.X);
                    SetTop(child, node.Pos.Y);
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
                        var viewport = MakeCurrentViewport();

                        foreach (var node in newItems)
                        {
                            var child = new BiaNodePanel {DataContext = node};

                            SetLeft(child, node.Pos.X);
                            SetTop(child, node.Pos.Y);
                            child.Width = 80;
                            child.Height = 80;

                            child.MouseEnter += (s, _) => SetFrontmost((BiaNodePanel) s);

                            _children.Add(node, child);

                            var childRect = new Rect(node.Pos.X, node.Pos.Y, child.Width, child.Height);

                            if (viewport.IntersectsWith(childRect))
                                Children.Add(child);
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
            Children.Remove(child);
            Children.Add(child);
        }

        private void UpdateScrollViewer(ScrollViewer oldScrollViewer, ScrollViewer newScrollViewer)
        {
            if (oldScrollViewer != null)
                oldScrollViewer.ScrollChanged -= OnScrollChanged;

            if (newScrollViewer != null)
                newScrollViewer.ScrollChanged += OnScrollChanged;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            MakeChildren();
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
                    if (Children.Contains(t) == false)
                        Children.Add(t);
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
                    if (Children.Contains(t))
                        Children.Remove(t);
            }
        }

        private Rect MakeCurrentViewport() =>
            new Rect(
                ScrollViewer.HorizontalOffset / Scale,
                ScrollViewer.VerticalOffset / Scale,
                ScrollViewer.ViewportWidth / Scale,
                ScrollViewer.ViewportHeight / Scale);
    }
}