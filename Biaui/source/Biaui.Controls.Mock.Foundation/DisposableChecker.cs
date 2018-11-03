using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Biaui.Controls.Mock.Foundation.Exception;
using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation
{
    public class DisposableChecker : IDisposableChecker
    {
        private ConcurrentDictionary<IDisposable, int> _Disposables;

        private ConcurrentDictionary<IDisposable, int> Disposables =>
            LazyInitializer.EnsureInitialized(ref _Disposables, () => new ConcurrentDictionary<IDisposable, int>());

        private Action<string> _showError;
        private int _single;

        public void Start(Action<string> showError)
        {
            var old = Interlocked.Exchange(ref _single, 1);
            if (old != 0)
                throw new NestingException();

            Disposables.Clear();
            _showError = showError;
        }

        public void End()
        {
            if (Disposables.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var d in Disposables.Keys)
                    sb.AppendLine("    " + d.GetType());

                _showError?.Invoke("Found undisposed object(s).\n" + sb);
            }

            Disposables.Clear();

            var old = Interlocked.Exchange(ref _single, 0);
            if (old != 1)
                throw new NestingException();
        }

        public void Clean()
        {
            _showError = null;
            _single = 0;
            Disposables.Clear();
        }

        public void Add(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable))
                _showError?.Invoke("Found multiple addition.    -- " + disposable.GetType());

            Disposables[disposable] = 0;
        }

        public void Remove(IDisposable disposable)
        {
            Debug.Assert(disposable != null);

            if (Disposables.ContainsKey(disposable) == false)
                _showError?.Invoke("Found multiple removing.    -- " + disposable.GetType());

            Disposables.TryRemove(disposable, out var dummy);
        }
    }
}