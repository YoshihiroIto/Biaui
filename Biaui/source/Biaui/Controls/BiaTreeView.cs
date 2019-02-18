using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTreeView : TreeView
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems), typeof(IList), typeof(BiaTreeView));

        public IList SelectedItems
        {
            get => (IList) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem), typeof(object), typeof(BiaTreeView),
            new FrameworkPropertyMetadata(OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var self = (BiaTreeView) obj;

            if (self.SelectedItems == null)
                return;

            self.SelectedItems.Clear();
            self.SelectedItems.Add(e.NewValue);
        }

        public new object SelectedItem
        {
            get => (TreeViewItem) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached(
            "IsSelected", typeof(bool), typeof(BiaTreeView),
            new FrameworkPropertyMetadata(OnIsSelectedChanged));


        public event EventHandler ItemSelectionStarting;
        public event EventHandler ItemSelectionCompleted;

        static BiaTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTreeView),
                new FrameworkPropertyMetadata(typeof(BiaTreeView)));
        }

        public static bool GetIsSelected(TreeViewItem target)
        {
            return (bool) target.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(TreeViewItem target, bool value)
        {
            target.SetValue(IsSelectedProperty, value);
        }

        private static void OnIsSelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var treeViewItem = obj as TreeViewItem;

            var parent = treeViewItem?.GetParent<BiaTreeView>();
            var selectedItems = parent?.SelectedItems;

            if (selectedItems == null)
                return;

            if (GetIsSelected(treeViewItem))
                selectedItems.Add(treeViewItem.DataContext);
            else
                selectedItems.Remove(treeViewItem.DataContext);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!(e.OriginalSource is TreeViewItem treeViewItem))
                return;

            TreeViewItem targetItem = null;

            switch (e.Key)
            {
                case Key.Down:
                    targetItem = GetRelativeItem(treeViewItem, 1);
                    break;

                case Key.Up:
                    targetItem = GetRelativeItem(treeViewItem, -1);
                    break;
            }

            if (targetItem == null)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                SelectMultipleItems(targetItem);
            else
                SelectSingleItem(targetItem);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            var orgSource = e.OriginalSource as FrameworkElement;
            if (orgSource == null)
                return;

            // アイテムの改変マークだったら選択処理を行わない
            var toggleButton = orgSource.GetParent<ToggleButton>();
            if (toggleButton != null)
                if (toggleButton.Name == Biaui.Internals.Constants.TreeViewItemExpanderName)
                    if (toggleButton.Visibility == Visibility.Visible)
                        return;

            var treeViewItem = orgSource.GetParent<TreeViewItem>();
            if (treeViewItem == null)
                return;

            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Control:
                    ToggleSingleItem(treeViewItem);
                    break;

                case ModifierKeys.Shift:
                    SelectMultipleItems(treeViewItem);
                    break;

                default:
                    SelectSingleItem(treeViewItem);
                    break;
            }
        }

        private void ToggleSingleItem(TreeViewItem treeViewItem)
        {
            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var current = GetIsSelected(treeViewItem);
                var next = !current;

                SetIsSelected(treeViewItem, next);

                SelectedItem = next ? treeViewItem.DataContext : null;
                _multipleSelectionEdgeItem = treeViewItem;
            }
            ItemSelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void SelectSingleItem(TreeViewItem treeViewItem)
        {
            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var items = this.EnumerateChildren<TreeViewItem>();

                foreach (var item in items)
                    SetIsSelected(item, item == treeViewItem);

                SelectedItem = treeViewItem.DataContext;
                _multipleSelectionEdgeItem = treeViewItem;
            }
            ItemSelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private TreeViewItem _multipleSelectionEdgeItem;

        private void SelectMultipleItems(TreeViewItem edgeItem)
        {
            if (_multipleSelectionEdgeItem == null)
                return;

            if (edgeItem == null)
                return;

            if (edgeItem == _multipleSelectionEdgeItem)
            {
                SelectSingleItem(edgeItem);
                return;
            }

            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var isInSelection = false;

                foreach (var item in this.EnumerateChildren<TreeViewItem>())
                {
                    if (ReferenceEquals(item, edgeItem) ||
                        ReferenceEquals(item, _multipleSelectionEdgeItem))
                    {
                        isInSelection = !isInSelection;

                        SetIsSelected(item, true);

                        if (isInSelection == false)
                            break;
                    }
                    else if (isInSelection)
                    {
                        SetIsSelected(item, true);
                    }
                }

                SelectedItem = edgeItem.DataContext;
                _multipleSelectionEdgeItem = edgeItem;
            }
            ItemSelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private T GetRelativeItem<T>(T item, int relativePosition)
            where T : ItemsControl
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var items = this.EnumerateChildren<T>().ToArray();
            var index = Array.IndexOf(items, item);

            if (index == -1)
                return null;

            var relativeIndex = index + relativePosition;
            if (relativeIndex >= 0 && relativeIndex < items.Length)
                return items[relativeIndex];

            return null;
        }
    }
}