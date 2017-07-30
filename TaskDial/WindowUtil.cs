using System.Runtime.InteropServices;

namespace TaskDial
{
    static class WindowUtil
    {
        private const string User32 = "user32.dll";

        public const int SwHide = 0;
        public const int SwMaximize = 3;
        public const int SwMinimize = 6;
        public const int SwRestore = 9;
        public const int SwShow = 5;
        public const int SwShowDefault = 10;
        public const int SwShowNormal = 1;

        [DllImport(User32)]
        public static extern bool SetForegroundWindow(int handle);

        [DllImport(User32)]
        public static extern bool ShowWindow(int handle, int cmd);

        [DllImport(User32)]
        public static extern bool IsIconic(int handle);

        public static void SwitchToWindow(int handle)
        {
            SetForegroundWindow(handle);
            if (IsIconic(handle))
            {
                ShowWindow(handle, SwRestore);
            }
        }
    }
}
