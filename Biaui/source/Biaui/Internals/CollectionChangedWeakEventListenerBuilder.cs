using System;
using System.Collections.Specialized;
using System.Windows;

namespace Biaui;

internal static class CollectionChangedWeakEventListenerBuilder
{
    public static INotifyCollectionChangedWeakEventListener Build(
        INotifyCollectionChanged notifyCollectionChanged,
        Action<NotifyCollectionChangedEventArgs> raiseCollectionChangedAction)
    {
        var listener = new CollectionChangedNotifyPropertyChangedWeakEventListener(
            notifyCollectionChanged,
            raiseCollectionChangedAction);

        CollectionChangedEventManager.AddListener(notifyCollectionChanged, listener);

        return listener;
    }

    private sealed class CollectionChangedNotifyPropertyChangedWeakEventListener :
        INotifyCollectionChangedWeakEventListener,
        IWeakEventListener
    {
        private readonly INotifyCollectionChanged _notifyCollectionChanged;
        private readonly Action<NotifyCollectionChangedEventArgs> _raiseCollectionChangedAction;

        public CollectionChangedNotifyPropertyChangedWeakEventListener(
            INotifyCollectionChanged notifyCollectionChanged,
            Action<NotifyCollectionChangedEventArgs> raiseCollectionChangedAction)
        {
            _notifyCollectionChanged = notifyCollectionChanged;
            _raiseCollectionChangedAction = raiseCollectionChangedAction;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(CollectionChangedEventManager))
                return false;

            if (e is not NotifyCollectionChangedEventArgs args)
                return false;

            _raiseCollectionChangedAction(args);

            return true;
        }

        public void Dispose()
        {
            CollectionChangedEventManager.RemoveListener(_notifyCollectionChanged, this);
        }
    }
}

internal interface INotifyCollectionChangedWeakEventListener : IDisposable
{
}