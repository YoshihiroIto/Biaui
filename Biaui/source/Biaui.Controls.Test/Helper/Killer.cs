using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Biaui.Controls.Test.Helper
{
    internal class Killer : IDisposable
    {
        public bool IsKilled { get; private set; }
        public bool IsTimeout { get; private set; }

        private readonly Process _process;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isCanceled;

        public Killer(int timeoutMs, Process process)
        {
            _process = process;

            Task.Run(async () =>
            {
                await Task.Delay(timeoutMs, _cts.Token).ConfigureAwait(false);

                if (_isCanceled == false)
                {
                    IsTimeout = true;

                    Kill();
                    IsKilled = true;
                }
            });
        }

        public void Dispose()
        {
            _isCanceled = true;

            _cts.Cancel();
            _cts.Dispose();
        }

        public void Kill()
        {
            _process.Kill(true);
        }
    }
}