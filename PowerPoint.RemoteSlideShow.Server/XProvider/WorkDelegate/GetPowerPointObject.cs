using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PPT = Microsoft.Office.Interop.PowerPoint;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.WorkDelegate
{
    public delegate PPT.Application GetPowerPointObject();
}
