using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Biaui.Internals
{
    public static class GuiHelper
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
}