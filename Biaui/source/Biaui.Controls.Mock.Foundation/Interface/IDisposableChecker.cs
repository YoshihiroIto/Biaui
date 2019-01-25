using System;

namespace Biaui.Controls.Mock.Foundation.Interface
{
    public interface IDisposableChecker
    {
        void Start(Action<string> showError);
        void End();
        void Clean();
        void Add(IDisposable disposable);
        void Remove(IDisposable disposable);
    }
}