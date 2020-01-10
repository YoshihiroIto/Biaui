using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class TreeViewItemExtensions
    {
        internal static int GetDepth(this TreeViewItem item)
        {
            try
            {
                TreeViewItem? parent;
                while ((parent = GetParent(item)) != null)
                {
                    return GetDepth(parent) + 1;
                }
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        private static TreeViewItem? GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent ?? throw new InvalidOperationException());
            }

            return parent as TreeViewItem;
        }
    }
}