using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Value
{
    public class NetworkValue
    {
        public static string LANIPAddress
        {
            get
            {
                IPAddress[] ipa = Dns.GetHostAddresses(Dns.GetHostName()).Where(((x) => (x.AddressFamily == AddressFamily.InterNetwork))).ToArray();

                return ((ipa.Length > 0) ? ipa[0].ToString() : String.Empty);
            }
        }

        public static int HTTPOK
        {
            get
            {
                return 200;
            }
        }

        public static int HTTPNotFound
        {
            get
            {
                return 404;
            }
        }
    }
}
