using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Value
{
    public class AssemblyValue
    {
        public static Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public static string Name
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static string Title
        {
            get
            {
                return (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute).Title;
            }
        }
    }
}
