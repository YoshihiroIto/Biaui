using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Biaui.Controls.Test.Helper
{
    internal class Killer : IDisposable
    {
        private readonly int _processId;
        public bool IsKilled { get; private set; }
        public bool IsTimeout { get; private set; }

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isCanceled;

        public Killer(int timeoutMs, int processId)
        {
            _processId = processId;
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
            KillProcessAndChildren(_processId);
        }

        // https://stackoverflow.com/questions/30249873/process-kill-doesnt-seem-to-kill-the-process
        private static void KillProcessAndChildren(int pid)
        {
            var processSearcher =
                new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            var processCollection = processSearcher.Get();

            try
            {
                var proc = Process.GetProcessById(pid);
                if (!proc.HasExited)
                    proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            foreach (var o in processCollection)
            {
                var mo = (ManagementObject) o;
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
        }
    }
}