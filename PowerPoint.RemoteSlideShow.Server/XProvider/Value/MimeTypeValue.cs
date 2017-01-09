using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Value
{
    public class MimeTypeValue
    {
        public static string PNGExtension
        {
            get
            {
                return "png";
            }
        }

        public static string PNG
        {
            get
            {
                return "image/png";
            }
        }

        public static string HTML
        {
            get
            {
                return "text/html";
            }
        }

        public static string XML
        {
            get
            {
                return "text/xml";
            }
        }

        public static string JavaScript
        {
            get
            {
                return "text/javascript";
            }
        }

        public static string StyleSheet
        {
            get
            {
                return "text/css";
            }
        }
    }
}
