using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Value
{
    public class MimeTypeValue
    {
        public static string sPNGExtension
        {
            get
            {
                return "png";
            }
        }

        public static string sPNGMimeType
        {
            get
            {
                return "image/png";
            }
        }

        public static string sHTMLMimeType
        {
            get
            {
                return "text/html";
            }
        }

        public static string sXMLMimeType
        {
            get
            {
                return "text/xml";
            }
        }

        public static string sJavaScriptMimeType
        {
            get
            {
                return "text/javascript";
            }
        }

        public static string sStyleSheetMimeType
        {
            get
            {
                return "text/css";
            }
        }
    }
}
