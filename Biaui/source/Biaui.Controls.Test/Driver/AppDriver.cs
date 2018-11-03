using System;
using System.Diagnostics;
using System.IO;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;

namespace Biaui.Controls.Test.Driver
{
    internal class AppDriver : IDisposable
    {
        private static string ExePath
        {
            get
            {
                const string exe =
#if DEBUG
                    "../../../../Biaui.Controls.Mock/bin/Debug/Biaui.Controls.Mock.exe";
#else
                    "../../../../Biaui.Controls.Mock/bin/Release/Biaui.Controls.Mock.exe";
#endif

                return Path.GetFullPath(exe);
            }
        }

        public void Dispose()
        {
            //タイムアップ終了していない場合は自ら終了させる
            if (_killer.IsKilled == false)
                _killer.Kill();

            _killer.Dispose();
            _app.Dispose();
            _proc.Dispose();
        }

        public WindowControl MainWindow => _mainWindowDriver.Window;

        private readonly Process _proc;
        private readonly WindowsAppFriend _app;
        private readonly MainWindowDriver _mainWindowDriver;
        private readonly Killer _killer;

        public AppDriver()
            : this(null, 5000)
        {
        }

        public AppDriver(string args, int timeoutMs)
        {
            _proc = Process.Start(ExePath, args);
            _app = new WindowsAppFriend(_proc);
            _mainWindowDriver = new MainWindowDriver(_app.Type<System.Windows.Application>().Current.MainWindow);

            // ReSharper disable once PossibleNullReferenceException
            _killer = new Killer(timeoutMs, _proc.Id);
        }
    }
}