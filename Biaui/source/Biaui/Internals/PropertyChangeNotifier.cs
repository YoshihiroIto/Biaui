using System;
using System.Windows;
using System.Windows.Data;

namespace Biaui.Internals
{
    // https://stackoverflow.com/questions/23682232/how-can-i-fix-the-dependencypropertydescriptor-addvaluechanged-memory-leak-on-at
    // https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/

    internal sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        private readonly WeakReference _propertySource;

        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException(nameof(propertySource));

            if (null == property)
                throw new ArgumentNullException(nameof(property));

            _propertySource = new WeakReference(propertySource);

            var binding = new Binding
            {
                Path = property,
                Mode = BindingMode.OneWay,
                Source = propertySource
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        public DependencyObject? PropertySource
        {
            get
            {
                try
                {
                    return _propertySource.IsAlive
                        ? _propertySource.Target as DependencyObject
                        : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var notifier = (PropertyChangeNotifier) d;
            notifier.ValueChanged?.Invoke(notifier, EventArgs.Empty);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public event EventHandler? ValueChanged;

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}