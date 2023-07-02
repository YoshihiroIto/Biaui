using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Biaui.Internals;

public static class ItemsControlExtensions
{
    internal static IEnumerable<T> EnumerateChildren<T>(this ItemsControl self)
        where T : ItemsControl
    {
        if (self is null)
            throw new ArgumentNullException(nameof(self));

        for (var i = 0; i < self.Items.Count; i++)
        {
            if (!(self.ItemContainerGenerator.ContainerFromIndex(i) is T item))
                continue;

            yield return item;

            foreach (var c in item.EnumerateChildren<T>())
                yield return c;
        }
    }       
}
