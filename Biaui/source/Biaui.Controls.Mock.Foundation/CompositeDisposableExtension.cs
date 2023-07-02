using System;
using System.Reactive.Disposables;

namespace Biaui.Controls.Mock.Foundation;

// ReSharper disable once UnusedMember.Global
public static class CompositeDisposableExtension
{
    public static void Add(this CompositeDisposable c, Action action)
    {
        c.Add(Disposable.Create(action));
    }
}