using System;
using System.Collections.Generic;
using System.Linq;
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
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is Popup popup)
            {
                if (popup.Child != null)
                    yield return popup.Child;
            }

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
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            foreach (var child in obj.Children())
            {
                yield return child;

                foreach (var grandChild in child.Descendants())
                    yield return grandChild;
            }
        }

        public static IEnumerable<T> Children<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Children().OfType<T>();
        }

        public static IEnumerable<T> Descendants<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Descendants().OfType<T>();
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