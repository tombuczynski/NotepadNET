using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tomproj.WPFUtils
{
    public static class ColorUtils
    {
        public static System.Drawing.Color Convert(this Color color)
            => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        public static Color Convert(this System.Drawing.Color wfColor)
            => Color.FromArgb(wfColor.A, wfColor.R, wfColor.G, wfColor.B);

        public static bool ChooseColorDlg(ref Color color)
        {
            using (System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog())
            {
                dialog.Color = color.Convert();
                dialog.AllowFullOpen = true;

                bool colorChoosed = dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
                if (colorChoosed)
                {
                    color = dialog.Color.Convert();
                }

                return colorChoosed;
            }
        }
    }
}
