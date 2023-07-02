using System;
using System.IO;
using Biaui.Controls.Test.Driver;
using Codeer.Friendly.Windows.Grasp;
using Xunit;

namespace Biaui.Controls.Test.Helper;

public class MockAppFixture : IDisposable
{
    public WindowControl MainWindow => _appDriver.MainWindow;

    public void Check()
    {
        // エラー、警告がない
        foreach (var line in ReadLogLines())
        {
            Assert.DoesNotContain("[ERROR]", line);
            Assert.DoesNotContain("[WARN ]", line);
        }
    }

    private string[] ReadLogLines()
        => File.ReadAllLines(Path.Combine(_configDir.RootPath, "Logs", DateTime.Today.ToString("yyyyMMdd") + ".log"));

    private readonly AppDriver _appDriver;
    private readonly TempDir _configDir = new TempDir();

    private string AppConfigFilePath
        => $"{_configDir.RootPath}/AppConfig.json";

    public MockAppFixture()
    {
        _appDriver = new AppDriver($"--config {AppConfigFilePath}", 10 * 60 * 1000);
    }

    public void Dispose()
    {
        _appDriver.MainWindow.Close();

        _appDriver.Dispose();
        _configDir.Dispose();
    }
}