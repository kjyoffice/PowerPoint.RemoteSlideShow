using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Model
{
    public class SlideItem
    {
        public string sExportFilePath { get; private set; }
        public string sMemo { get; private set; }

        // --------------------------------------

        public SlideItem(string sExportFilePath, string sMemo)
        {
            this.sExportFilePath = sExportFilePath;
            this.sMemo = sMemo;
        }
    }
}
