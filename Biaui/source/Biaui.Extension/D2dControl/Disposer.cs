using System;

namespace D2dControl
{
    internal static class Disposer
    {
        internal static void SafeDispose<T>(ref T resource) where T : class
        {
            if (resource == null)
                return;

            if (resource is IDisposable disposer)
            {
                try
                {
                    disposer.Dispose();
                }
                catch
                {
                    // ignored
                }
            }

            resource = null;
        }
    }
}