using System;
using System.Collections.Generic;

namespace Biaui.Controls.Mock.Foundation
{
    public static class DisposableExtensions
    {
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container) where T : IDisposable
        {
            container.Add(disposable);
            return disposable;
        }
    }
}