using System;
using System.IO;
using System.Linq;
using Biaui.Controls.Test.Driver;
using Biaui.Controls.Test.Helper;
using Xunit;

namespace Biaui.Controls.Test
{
    public class SmokeTest : IDisposable
    {
        public void Dispose()
            => _configDir?.Dispose();

        private readonly TempDir _configDir = new TempDir();

        private string[] ReadLog()
            => File.ReadAllLines(Path.Combine(_configDir.RootPath, "Logs", DateTime.Today.ToString("yyyyMMdd") + ".log"));

        private string AppConfigFilePath
            => $"{_configDir.RootPath}/AppConfig.json";

        /// <summary>
        /// 無事開始して終わるか
        /// </summary>
        [Fact]
        public void StartEnd()
        {
            using (var appDriver = new AppDriver($"--config {AppConfigFilePath}", 5000))
            {
                appDriver.MainWindow.Close();
            }

            Assert.True(File.Exists(AppConfigFilePath));

            var log = ReadLog();
            Assert.Contains("Start Application", log.First());
            Assert.Contains("End Application", log.Last());
        }
    }
}