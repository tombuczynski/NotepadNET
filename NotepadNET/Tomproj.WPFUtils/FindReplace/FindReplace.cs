using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomproj.WPFUtils.FindReplace
{
    public enum FindReplaceMode
    {
        FindNext, FindPrev, ReplaceNext, ReplaceAll
    }

    public class FindReplaceEventArgs : EventArgs
    {
        public FindReplaceMode Mode { get; set; }
        public string TextToFind { get; set; }
        public string TextToReplace { get; set; }
        public bool CaseSensitive { get; set; }
    }

}
