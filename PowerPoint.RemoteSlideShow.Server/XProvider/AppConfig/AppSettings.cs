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
        public static string SlideExportDirectoryPath
        {
            get
            {
                string sValue = ConfigurationManager.AppSettings["SlideExportDirectoryPath"].Trim();
                string sResult = (((sValue == String.Empty) || ((sValue != String.Empty) && (Directory.Exists(sValue) == false))) ? Environment.CurrentDirectory : sValue);

                return sResult;
            }
        }

        public static int SingleServerPortNo
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["SingleServerPortNo"].Trim());
            }
        }

        public static string SingleServerRootDirectoryName
        {
            get
            {
                return ConfigurationManager.AppSettings["SingleServerRootDirectoryName"].Trim();
            }
        }
    }
}
