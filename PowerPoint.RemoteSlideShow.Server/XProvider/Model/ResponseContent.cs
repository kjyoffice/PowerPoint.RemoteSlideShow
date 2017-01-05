using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Model
{
    public class ResponseContent
    {
        public int iStatusCode { get; set; }
        public string sStatusDescription { get; set; }
        public string sContentType { get; set; }
        public byte[] btBuffer { get; set; }

        // ---------------------------------------

        public ResponseContent(int iStatusCode, string sContentType, string sResponseText)
            : this(iStatusCode, sContentType, Encoding.UTF8.GetBytes(sResponseText))
        {
            //>
        }

        public ResponseContent(int iStatusCode, string sContentType, byte[] btBuffer)
        {
            this.iStatusCode = iStatusCode;
            this.sStatusDescription = ((iStatusCode == Value.NetworkValue.iHTTPStatusCode_OK) ? "OK" : "Not Found");
            this.sContentType = sContentType;
            this.btBuffer = btBuffer;
        }
    }
}
