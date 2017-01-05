using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.AppConfig
{
    public class AppSettings
    {
        public static string sSlideExportDirectoryPath
        {
            get
            {
                string sValue = ConfigurationManager.AppSettings["SlideExportDirectoryPath"].Trim();
                string sResult = (((sValue == String.Empty) || ((sValue != String.Empty) && (Directory.Exists(sValue) == false))) ? Environment.CurrentDirectory : sValue);

                return sResult;
            }
        }

        public static int iSingleServerPortNo
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["SingleServerPortNo"].Trim());
            }
        }

        public static string sSingleServerRootDirectoryName
        {
            get
            {
                return ConfigurationManager.AppSettings["SingleServerRootDirectoryName"].Trim();
            }
        }
    }
}
