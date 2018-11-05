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

        private string[] ReadLogLines()
            => File.ReadAllLines(Path.Combine(_configDir.RootPath, "Logs", DateTime.Today.ToString("yyyyMMdd") + ".log"));

        private string AppConfigFilePath
            => $"{_configDir.RootPath}/AppConfig.json";

        /// <summary>
        /// 開始して無事終わるか
        /// </summary>
        [Fact]
        public void StartEnd()
        {
            using (var appDriver = new AppDriver($"--config {AppConfigFilePath}", 5000))
            {
                appDriver.MainWindow.Close();
            }

            Assert.True(File.Exists(AppConfigFilePath));

            var lines = ReadLogLines();
            Assert.Contains("Start Application", lines.First());
            Assert.Contains("End Application", lines.Last());

            // エラー、警告がない
            foreach (var line in lines)
            {
                Assert.DoesNotContain("[ERROR]", line);
                Assert.DoesNotContain("[WARN ]", line);
            }
        }
    }
}