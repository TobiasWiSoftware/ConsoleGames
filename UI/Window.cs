using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Chess.UI
{
    public class Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        const int SW_MAXIMIZE = 3;
        public static async Task MaximizeConsoleWindow()
        {
            IntPtr consoleWindowHandle = GetForegroundWindow();
            SetForegroundWindow(consoleWindowHandle);
            ShowWindow(consoleWindowHandle, SW_MAXIMIZE);

            await Task.Delay(500);
        }
    }
}
