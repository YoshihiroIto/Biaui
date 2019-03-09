using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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

            self._selectedItem = e.NewValue;
        }

        private object _selectedItem;

        public new object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                    SetValue(SelectedItemProperty, value);
            }
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

            if (parent == null)
                return;

            var selectedItems = parent.SelectedItems;

            var isSelected = GetIsSelected(treeViewItem);

            if (selectedItems != null)
            {
                if (isSelected)
                    selectedItems.Add(treeViewItem.DataContext);
                else
                    selectedItems.Remove(treeViewItem.DataContext);
            }

            if (isSelected)
                parent.SelectedItem = treeViewItem.DataContext;
            else
            {
                if (selectedItems == null || selectedItems.Count == 0)
                    parent.SelectedItem = null;
                else
                    parent.SelectedItem = selectedItems[0];
            }
        }

        private INotifyCollectionChanged _oldItemsSource;

        public BiaTreeView()
        {
            var itemsSourceDescriptor =
                DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(BiaTreeView));
            itemsSourceDescriptor.AddValueChanged(this,
                (sender, e) =>
                {
                    if (_oldItemsSource != null)
                        _oldItemsSource.CollectionChanged -= ItemsSourceOnCollectionChanged;

                    var itemsSource = ItemsSource as INotifyCollectionChanged;
                    if (itemsSource != null)
                        itemsSource.CollectionChanged += ItemsSourceOnCollectionChanged;

                    _oldItemsSource = itemsSource;
                });
        }

        private void ItemsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var vm = e.NewItems[0];

                    var item = this.EnumerateChildren<TreeViewItem>().FirstOrDefault(x => x.DataContext == vm);
                    item?.BringIntoView();

                    _multipleSelectionEdgeItemDataContext = vm;

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                    if (SelectedItems != null)
                    {
                        foreach (var item in e.OldItems)
                            SelectedItems.Remove(item);

                        SelectedItem = SelectedItems.Count == 0 ? null : SelectedItems[0];
                    }

                    _multipleSelectionEdgeItemDataContext = SelectedItem;

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!(e.OriginalSource is TreeViewItem treeViewItem))
                return;

            // [CTRL] + A
            if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SelectAllItems();
                e.Handled = true;
            }

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
            {
                e.Handled = true;
                return;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                SelectMultipleItems(targetItem);
            else
                SelectSingleItem(targetItem);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            OnPreviewMouseLeftButton(e, true);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            OnPreviewMouseLeftButton(e, false);
        }

        private void OnPreviewMouseLeftButton(MouseButtonEventArgs e, bool isDown)
        {
            if (!(e.OriginalSource is FrameworkElement orgSource))
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
                    if (isDown)
                        ToggleSingleItem(treeViewItem);
                    break;

                case ModifierKeys.Shift:
                    if (isDown)
                        SelectMultipleItems(treeViewItem);
                    break;

                default:
                    if (isDown)
                    {
                        if (GetIsSelected(treeViewItem) == false)
                            SelectSingleItem(treeViewItem);
                    }
                    else
                        SelectSingleItem(treeViewItem);

                    break;
            }
        }

        private void SelectAllItems()
        {
            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var items = this.EnumerateChildren<TreeViewItem>();

                TreeViewItem firstItem = null;

                foreach (var item in items)
                {
                    if (firstItem == null)
                        firstItem = item;

                    SetIsSelected(item, true);
                }

                if (firstItem != null)
                {
                    SelectedItem = firstItem.DataContext;
                    _multipleSelectionEdgeItemDataContext = firstItem.DataContext;
                }
            }
            ItemSelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void ToggleSingleItem(TreeViewItem treeViewItem)
        {
            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var current = GetIsSelected(treeViewItem);
                var next = !current;

                SetIsSelected(treeViewItem, next);

                SelectedItem = next ? treeViewItem.DataContext : null;
                _multipleSelectionEdgeItemDataContext = treeViewItem.DataContext;
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
                _multipleSelectionEdgeItemDataContext = treeViewItem.DataContext;
            }
            ItemSelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private object _multipleSelectionEdgeItemDataContext;

        private void SelectMultipleItems(TreeViewItem edgeItem)
        {
            if (_multipleSelectionEdgeItemDataContext == null)
                return;

            if (edgeItem == null)
                return;

            if (edgeItem.DataContext == _multipleSelectionEdgeItemDataContext)
            {
                SelectSingleItem(edgeItem);
                return;
            }

            ItemSelectionStarting?.Invoke(this, EventArgs.Empty);
            {
                var isInSelection = false;

                foreach (var item in this.EnumerateChildren<TreeViewItem>())
                {
                    if (ReferenceEquals(item.DataContext, edgeItem.DataContext) ||
                        ReferenceEquals(item.DataContext, _multipleSelectionEdgeItemDataContext))
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
                _multipleSelectionEdgeItemDataContext = edgeItem.DataContext;
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