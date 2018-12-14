using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
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

            //Debug.WriteLine($"RemoveChild>>>>>>>>>>>>>>{_children.Count}");
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
                child.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
    }
}