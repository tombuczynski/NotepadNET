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
    }
}
