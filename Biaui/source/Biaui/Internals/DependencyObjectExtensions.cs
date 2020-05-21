using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Biaui.Internals
{
    // http://blog.xin9le.net/entry/2013/10/29/222336 を参考にしました

    internal static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> Children(this DependencyObject obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is Popup popup)
                if (popup.Child != null)
                    yield return popup.Child;

            var count = VisualTreeHelper.GetChildrenCount(obj);
            if (count == 0)
                yield break;

            for (var i = 0; i != count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                yield return child;
            }
        }

        public static IEnumerable<DependencyObject> Descendants(this DependencyObject obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            var children = ArrayPool<DependencyObject>.Shared.Rent(obj.MaximumChildrenCount());

            try
            {
                var childrenCount = obj.Children(children);

                for (var i = 0; i != childrenCount; ++i)
                {
                    var child = children[i];

                    yield return child;

                    foreach (var grandChild in child.Descendants())
                        yield return grandChild;
                }
            }
            finally
            {
                ArrayPool<DependencyObject>.Shared.Return(children);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MaximumChildrenCount(this DependencyObject obj)
        {
            return VisualTreeHelper.GetChildrenCount(obj) + 1;
        }

        private static int Children(this DependencyObject obj, Span<DependencyObject> result)
        {
            var count = 0;

            if (obj is Popup popup)
                if (popup.Child != null)
                    result[count++] = popup.Child;

            var childrenCount = VisualTreeHelper.GetChildrenCount(obj);

            for (var i = 0; i != childrenCount; i++)
                result[count++] = VisualTreeHelper.GetChild(obj, i);

            return count;
        }

        public static T? GetParent<T>(this DependencyObject self) where T : class
        {
            var parent = self;

            do
            {
                if (parent is T tp)
                    return tp;

                parent = VisualTreeHelper.GetParent(parent);
            } while (parent != null);

            return null;
        }

        public static DependencyObject? GetParent(this DependencyObject self, Type type)
        {
            var parent = self;

            do
            {
                if (parent.GetType().IsSubclassOf(type))
                    return parent;

                parent = VisualTreeHelper.GetParent(parent);
            } while (parent != null);

            return null;
        }
    }
}