using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomproj.WPFUtils
{
    public static class FolderUtils
    {
        public static bool ChooseFolderDlg(ref string folder, string descr = null, bool showNewFolderButton = true)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = folder;
                dialog.ShowNewFolderButton = showNewFolderButton;
                if (!string.IsNullOrEmpty(descr))
                    dialog.Description = descr;

                bool folderChoosed = dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
                if (folderChoosed)
                {
                    folder = dialog.SelectedPath;
                }

                return folderChoosed;
            }
        }
    }
}
