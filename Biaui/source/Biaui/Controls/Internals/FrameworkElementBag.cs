using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;

namespace Biaui.Controls.Internals
{
    internal class FrameworkElementBag<T> : FrameworkElement
        where T : FrameworkElement
    {
        private readonly List<T> _children = new List<T>();
        private readonly HashSet<T> _childrenForSearch = new HashSet<T>();
        private readonly List<T> _changedElements = new List<T>();

        public IReadOnlyList<T> Children => _children;

        static FrameworkElementBag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameworkElementBag<T>),
                new FrameworkPropertyMetadata(typeof(FrameworkElementBag<T>)));
        }

        internal FrameworkElementBag(IHasTransform transform)
        {
            var g = new TransformGroup();
            g.Children.Add(transform.Scale);
            g.Children.Add(transform.Translate);
            RenderTransform = g;
        }

        internal void AddChild(T child)
        {
            if (_childrenForSearch.Contains(child))
                return;

            _children.Add(child);
            _childrenForSearch.Add(child);

            AddVisualChild(child);
            ChangeElement(child);
        }

        internal void RemoveChild(T child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            _childrenForSearch.Remove(child);

            RemoveVisualChild(child);
        }

        internal void ToLast(T child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            RemoveVisualChild(child);

            _children.Add(child);
            AddVisualChild(child);

            ChangeElement(child);
        }

        internal void ChangeElement(T child)
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
                var pos = ((IBiaHasPos) child.DataContext).AlignedPos();

                child.Arrange(new Rect(pos, child.DesiredSize));
            }

            _changedElements.Clear();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in _changedElements.ToArray())
            {
                child.Measure(availableSize);

                if (child.DataContext is IBiaNodeItem nodeItem)
                    nodeItem.Size = child.DesiredSize;
            }

            return base.MeasureOverride(availableSize);
        }
    }
}