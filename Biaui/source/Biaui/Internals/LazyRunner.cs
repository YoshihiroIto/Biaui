using System;
using System.Windows.Threading;

namespace Biaui.Internals
{
    internal class LazyRunner : IDisposable
    {
        private readonly Action _action;
        private DispatcherTimer _timer;

        public void Dispose()
        {
            _timer?.Stop();
        }

        internal LazyRunner(Action action)
        {
            _action = action;
        }

        internal void Run()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                _timer = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(500),
                    DispatcherPriority.ApplicationIdle,
                    (_, __) =>
                    {
                        _action();
                        _timer?.Stop();
                    }, Dispatcher.CurrentDispatcher);
            }
        }
    }
}