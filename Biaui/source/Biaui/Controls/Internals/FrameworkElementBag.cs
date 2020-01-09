using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Biaui.Controls.Internals
{
    internal class FrameworkElementBag<T> : FrameworkElement
        where T : FrameworkElement
    {
        internal IReadOnlyList<T> Children => _children;
        
        private readonly List<T> _children = new List<T>();
        private readonly HashSet<T> _childrenForSearch = new HashSet<T>();
        private readonly HashSet<T> _changedElements = new HashSet<T>();

        static FrameworkElementBag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameworkElementBag<T>),
                new FrameworkPropertyMetadata(typeof(FrameworkElementBag<T>)));
        }

        internal FrameworkElementBag(IHasTransform transform)
        {
            var g = new TransformGroup();
            g.Children.Add(transform.ScaleTransform);
            g.Children.Add(transform.TranslateTransform);
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

            child.Width = double.NaN;
            child.Height = double.NaN;
        }

        internal void ToLast(T child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            if (_children.Count == 0)
                return;

            if (Equals(_children[^1], child))
                return;

            _children.Remove(child);
            RemoveVisualChild(child);

            _children.Add(child);
            AddVisualChild(child);

            ChangeElement(child);
        }

        internal void ChangeElement(T child)
        {
            if (_isInMeasureOverride)
                return;

            _changedElements.Add(child);
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeChildren(_changedElements);

            _changedElements.Clear();

            return base.ArrangeOverride(finalSize);
        }

        private bool _isInMeasureOverride;

        protected override Size MeasureOverride(Size availableSize)
        {
            _isInMeasureOverride = true;

            MeasureChildren(_changedElements, availableSize);

            _isInMeasureOverride = false;

            return base.MeasureOverride(availableSize);
        }

        protected virtual void ArrangeChildren(IEnumerable<T> children)
        {
            foreach (var child in children)
                child.Arrange(new Rect(new Point(), child.DesiredSize));
        }

        protected virtual void MeasureChildren(IEnumerable<T> children, Size availableSize)
        {
            foreach (var child in children)
                child.Measure(availableSize);
        }
    }
}