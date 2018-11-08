using System;
using System.Runtime.InteropServices;

namespace Biaui.Internals
{
    public static class Win32Helper
    {
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ClipCursor(IntPtr ptr);

        public struct RECT
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
    }
}