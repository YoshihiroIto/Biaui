using System;
using System.Runtime.InteropServices;

namespace Biaui.Internals
{
    internal static class Win32Helper
    {
        [DllImport("User32.dll")]
        internal static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        internal static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern bool ClipCursor(IntPtr ptr);

        [DllImport("User32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(int vKey);

        internal const int VK_CONTROL = 0x11;
        internal const int VK_SPACE = 0x20;
    }
}