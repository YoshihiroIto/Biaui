using System;
using Biaui.Controls.Mock.Foundation;
using Biaui.Controls.Mock.Foundation.Interface;
using SimpleInjector;

namespace Biaui.Controls.Mock;

public class App : IDisposable
{
    private readonly AppConfig _appConfig;
    private readonly Container _dic;
    private readonly ILogger _logger;

    private string? _appConfigFilePath;
    private LocalStorageRepository<AppConfig>? _configRepos;

    public App(Container dic, ILogger logger, AppConfig appConfig)
    {
        _dic = dic;
        _logger = logger;
        _appConfig = appConfig;

        _logger.Info("Start Application");
    }

    public void Dispose()
    {
        _ = _configRepos ?? throw new InvalidOperationException();
        
        _appConfig.Save(_configRepos);

        _logger.Info("End Application");
    }

    public void Setup(CommandLine commandLine)
    {
        _appConfigFilePath = commandLine.AppConfigFilePath;

        _configRepos = _dic.GetInstance<LocalStorageRepository<AppConfig>>().Setup(_appConfigFilePath);

        _appConfig.Load(_configRepos);
    }
}