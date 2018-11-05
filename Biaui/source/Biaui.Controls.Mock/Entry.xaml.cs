using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Text;
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

            SetupLog();

            MainWindow = new MainWindow { DataContext = _serviceProvider.GetInstance<MainWindowViewModel>() };
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.GetInstance<IDisposableChecker>().End();
            _serviceProvider.Dispose();

            base.OnExit(e);
        }

        private void SetupLog()
        {
            Trace.Listeners.Add(new WpfTraceListener("Trace", _serviceProvider.GetInstance<ILogger>()));
            Debug.Listeners.Add(new WpfTraceListener("Debug", _serviceProvider.GetInstance<ILogger>()));

            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new WpfTraceListener("WPF", _serviceProvider.GetInstance<ILogger>()));
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error | SourceLevels.Warning;
        }

        private class WpfTraceListener : TraceListener
        {
            private readonly string _sign;
            private readonly ILogger _logger;
            private readonly StringBuilder _sb = new StringBuilder();

            public WpfTraceListener(string sign, ILogger logger)
            {
                _sign = sign;
                _logger = logger;
            }
            
            public override void Write(string message)
            {
                _sb.Append(message);
            }

            public override void WriteLine(string message)
            {
                _sb.Append(message);

                var line = _sb.ToString();

                if (line.IndexOf("error", StringComparison.InvariantCultureIgnoreCase) != -1)
                    _logger.Error("[" + _sign + "]: " + line);

                else
                if (line.IndexOf("warn", StringComparison.InvariantCultureIgnoreCase) != -1)
                    _logger.Warn("[" + _sign + "]: " + line);

                else
                    _logger.Trace("[" + _sign + "]: " + line);

                _sb.Clear();
            }
        }
    }
}
