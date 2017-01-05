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
        public static string sLANIPAddress
        {
            get
            {
                IPAddress[] ipaIP = Dns.GetHostAddresses(Dns.GetHostName()).Where(((x) => (x.AddressFamily == AddressFamily.InterNetwork))).ToArray();
                string sResult = ((ipaIP.Length > 0) ? ipaIP[0].ToString() : String.Empty);

                return sResult;
            }
        }

        public static int iHTTPStatusCode_OK
        {
            get
            {
                return 200;
            }
        }

        public static int iHTTPStatusCode_NotFound
        {
            get
            {
                return 404;
            }
        }
    }
}
