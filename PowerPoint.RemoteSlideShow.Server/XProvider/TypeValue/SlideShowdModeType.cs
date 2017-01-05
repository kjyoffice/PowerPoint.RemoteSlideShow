using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.TypeValue
{
    public enum SlideShowdModeType
    {
        Initialize,
        CreateWorkID,
        CheckSlide,
        ExportSlide,
        PreSetting,
        Ready,
        Start,
        End,
        Error
    }
}
