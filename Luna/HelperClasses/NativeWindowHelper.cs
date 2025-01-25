using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Luna.HelperClasses
{
    internal static class NativeWindowHelper
    {
        public static int SW_MAXIMISE = 3, SW_MINIMISE = 6, SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);
    }
}
