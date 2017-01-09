using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Model
{
    public class ResponseContent
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string ContentType { get; set; }
        public byte[] OutputBuffer { get; set; }

        // ---------------------------------------

        public ResponseContent(int statusCode, string contentType, string responseText)
            : this(statusCode, contentType, Encoding.UTF8.GetBytes(responseText))
        {
            //>
        }

        public ResponseContent(int statusCode, string contentType, byte[] outputBuffer)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = ((statusCode == Value.NetworkValue.HTTPOK) ? "OK" : "Not Found");
            this.ContentType = contentType;
            this.OutputBuffer = outputBuffer;
        }
    }
}
