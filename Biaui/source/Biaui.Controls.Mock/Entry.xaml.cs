using System;
using System.IO;
using System.Runtime;
using System.Windows;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Presentation;

namespace Biaui.Controls.Mock
{
    /// <summary>
    /// Entry.xaml の相互作用ロジック
    /// </summary>
    public partial class Entry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            _commandLine = CommandLine.Parse(args);

            _configDir = Path.GetDirectoryName(_commandLine.AppConfigFilePath);
            if (_configDir != null)
            {
                Directory.CreateDirectory(_configDir);
                ProfileOptimization.SetProfileRoot(_configDir);
                ProfileOptimization.StartProfile("Startup.Profile");
            }

            var e = new Entry();
            e.InitializeComponent();
            e.Run();
        }

        private static CommandLine _commandLine;
        private static string _configDir;

        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _serviceProvider = new ServiceProvider(_configDir);
            _serviceProvider.GetInstance<IDisposableChecker>().Start(m => MessageBox.Show(m));
            _serviceProvider.GetInstance<App>().Setup(_commandLine);

            MainWindow = new MainWindow { DataContext = _serviceProvider.GetInstance<MainWindowViewModel>() };
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.GetInstance<IDisposableChecker>().End();
            _serviceProvider.Dispose();

            base.OnExit(e);
        }
    }
}
