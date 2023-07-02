using System.Runtime.InteropServices;

namespace Biaui.Internals;

internal static class GuiHelper
{
    public static void ShowCursor()
    {
        while (ShowCursor(true) < 0)
        {
        }
    }

    public static void HideCursor()
    {
        while (ShowCursor(false) >= 0)
        {
        }
    }

    [DllImport("user32.dll")]
    private static extern int ShowCursor(bool bShow);
}