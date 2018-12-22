namespace Biaui.Internals
{
    internal static class KeyboardHelper
    {
        internal static bool IsPressSpace =>
            (Win32Helper.GetAsyncKeyState(Win32Helper.VK_SPACE) & 0x8000) != 0;

        internal static bool IsPressControl =>
            (Win32Helper.GetAsyncKeyState(Win32Helper.VK_CONTROL) & 0x8000) != 0;
    }
}