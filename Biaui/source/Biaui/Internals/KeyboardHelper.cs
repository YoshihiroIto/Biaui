namespace Biaui.Internals
{
    public static class KeyboardHelper
    {
        public static bool IsPressSpace =>
            (Win32Helper.GetAsyncKeyState(Win32Helper.VK_SPACE) & 0x8000) != 0;

        public static bool IsPressControl =>
            (Win32Helper.GetAsyncKeyState(Win32Helper.VK_CONTROL) & 0x8000) != 0;
    }
}