using System;
using System.Reactive.Disposables;
using System.Threading;
using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation.Mvvm
{
    public class DisposableNotificationObject : NotificationObject, IDisposable
    {
        private readonly IDisposableChecker _disposableChecker;
        public EventHandler Disposing;

        private CompositeDisposable _Trashes;

        public CompositeDisposable Trashes
        {
            get { return LazyInitializer.EnsureInitialized(ref _Trashes, () => new CompositeDisposable()); }
        }

        private bool _disposed;

        public DisposableNotificationObject(IDisposableChecker disposableChecker)
        {
            _disposableChecker = disposableChecker;

            _disposableChecker.Add(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Disposing?.Invoke(this, EventArgs.Empty);

            if (disposing)
                _Trashes?.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            _disposableChecker.Remove(this);
        }
    }
}