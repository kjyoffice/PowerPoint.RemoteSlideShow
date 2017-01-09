using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Model
{
    public class SlideItem
    {
        public string ExportFilePath { get; private set; }
        public string Memo { get; private set; }

        // --------------------------------------

        public SlideItem(string exportFilePath, string memo)
        {
            this.ExportFilePath = exportFilePath;
            this.Memo = memo;
        }
    }
}
