using Codeer.Friendly.Windows.Grasp;

namespace Biaui.Controls.Test.Driver;

internal class MainWindowDriver
{
    public WindowControl Window { get; }

    public MainWindowDriver(dynamic window)
    {
        Window = new WindowControl(window);
    }
}