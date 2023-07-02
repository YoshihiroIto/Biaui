using Biaui.Controls.Mock.Foundation;
using Biaui.Controls.Mock.Foundation.Interface;
using SimpleInjector;
using ILogger = Biaui.Controls.Mock.Foundation.Interface.ILogger;

namespace Biaui.Controls.Mock;

public class ServiceProvider : Container
{
    public ServiceProvider(string outputDir)
    {
        SetupContainer();

        ////////////////////////////////////////////////////////////////////////////////////////
        void SetupContainer()
        {
            RegisterSingleton<App>();
            RegisterSingleton<AppConfig>();
            RegisterSingleton<LocalStorage>();
            RegisterSingleton<ILogger>(() => new Logger(outputDir));

            RegisterSingleton<IDisposableChecker>(() =>
#if DEBUG
                    new DisposableChecker()
#else
                    new NullDisposableChecker()
#endif
            );

#if DEBUG
            Verify();
#endif
        }
    }
}