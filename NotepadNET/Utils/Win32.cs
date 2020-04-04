using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tomproj.WPFUtils
{
    public class Win32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MessageBox(int hWnd, String text,
                        String caption, uint type);

        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int GWL_STYLE = -16;
        public const int WS_MAXIMIZEBOX = 0x10000;
    }
}
