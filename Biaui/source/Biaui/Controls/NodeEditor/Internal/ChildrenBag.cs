using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class ChildrenBag : FrameworkElement
    {
        private readonly List<FrameworkElement> _children = new List<FrameworkElement>();
        private readonly HashSet<FrameworkElement> _childrenForSearch = new HashSet<FrameworkElement>();
        private readonly List<FrameworkElement> _changedElements = new List<FrameworkElement>();

        static ChildrenBag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildrenBag),
                new FrameworkPropertyMetadata(typeof(ChildrenBag)));
        }

        internal void AddChild(FrameworkElement child)
        {
            if (_childrenForSearch.Contains(child))
                return;

            _children.Add(child);
            _childrenForSearch.Add(child);

            AddVisualChild(child);
            ChangeElement(child);
        }

        internal void RemoveChild(FrameworkElement child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            _childrenForSearch.Remove(child);

            RemoveVisualChild(child);

            //Debug.WriteLine($"RemoveChild>>>>>>>>>>>>>>{_children.Count}");
        }

        internal void ToLast(FrameworkElement child)
        {
            if (_childrenForSearch.Contains(child) == false)
                return;

            _children.Remove(child);
            RemoveVisualChild(child);

            _children.Add(child);
            AddVisualChild(child);
        }

        internal void ChangeElement(FrameworkElement child)
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
                Point pos;

                if (child.DataContext is INodeItem vm)
                    pos = vm.Pos;
                else if (child is BoxSelector)
                    pos = new Point(0, 0);
                else
                    throw new NotSupportedException();

                child.Arrange(new Rect(pos, child.DesiredSize));
            }

            _changedElements.Clear();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in _changedElements)
                child.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
    }
}