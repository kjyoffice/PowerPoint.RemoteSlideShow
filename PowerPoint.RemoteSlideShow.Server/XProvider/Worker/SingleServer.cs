using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PowerPoint.RemoteSlideShow.Server.XProvider.Worker
{
    // http://www.sysnet.pe.kr/Default.aspx?mode=2&sub=0&pageno=0&detail=1&wid=1555

    // 주) HttpListener가 리스닝을 시작하기 위해서는 관리자 권한을 필요로 한다. 
    // Windows Vista 에서 UAC(User Account Control)이 적용되고 있다면 "액세스 거부" 예외가 발생할 것이다. 
    // 이를 피하기 위해서는 manifest 를 작성해야만 한다. 관련 글을 참고하기 바란다.
    // http://www.simpleisbest.net/archive/2007/01/08/1527.aspx

    // 그렇습니다. 
    // netsh을 이용해 http.sys가 전달할 URL에 대한 매핑 정보를 관리할 수 있습니다. 
    // 즉, 관리자 권한으로 한번만 매핑 정보를 등록해 주면 그 이후로는 응용 프로그램을 일반 사용자 계정으로 실행해도 됩니다. 
    // 이 글의 예제 프로그램의 경우에는 다음과 같이 등록해 주어야겠지요. ^^
    /*
        형식: netsh http add urlacl url=[URL 매핑 정보] user=[Application 실행 계정]

        예: netsh http add urlacl url=http://+:80/MyTemp/ user="SeongTae Jeong"
           
        삭제 : netsh http delete urlacl url=http://+:80/MyTemp/ 
    */

    public class SingleServer
    {
        private WorkDelegate.dgGetWorkID dgWorkID { get; set; }
        private WorkDelegate.dgGetSlideItem dgSlideItem { get; set; }
        private WorkDelegate.dgGetDocumentName dgDocumentName { get; set; }
        private WorkDelegate.dgGetSlideSize dgSlideSize { get; set; }
        private WorkDelegate.dgGetTotalSlideCount dgSlideCount { get; set; }
        private WorkDelegate.dgSetSlideShowCommand dgSlideShowCommand { get; set; }
        private WorkDelegate.dgGetNowErrorMode dgErrorMode { get; set; }
        private WorkDelegate.dgSetNotifyErrorMode dgNotifyError { get; set; }
        private HttpListener hlHttp { get; set; }
        private int iPort { get; set; }
        private string sURLRootDirectoryName { get; set; }
        public string sConnectPassword { get; private set; }
        private string sConnectPassword_Upper { get; set; }

        // ------------------------------------------

        public string sURL
        {
            get
            {
                string sLanIP = Value.NetworkValue.sLANIPAddress;
                string sPort = ((this.iPort != 80) ? (":" + this.iPort) : String.Empty);
                string sResult = ("http://" + (sLanIP + sPort) + "/" + this.sURLRootDirectoryName + "/");

                return sResult;
            }
        }

        // ------------------------------------------

        private void ProcessRequest(IAsyncResult arInBound)
        {
            HttpListener hlHttp = (arInBound.AsyncState as HttpListener);
            HttpListenerContext hlcContext;
            HttpListenerResponse hlrResponse;
            Model.ResponseContent rcResponse;

            if ((hlHttp != null) && (hlHttp.IsListening == true))
            {
                hlcContext = hlHttp.EndGetContext(arInBound);

                rcResponse = this.SelectResponseContent(hlcContext.Request);

                hlrResponse = hlcContext.Response;
                hlrResponse.StatusCode = rcResponse.iStatusCode;
                hlrResponse.StatusDescription = rcResponse.sStatusDescription;
                hlrResponse.ContentType = rcResponse.sContentType;
                hlrResponse.OutputStream.Write(rcResponse.btBuffer, 0, rcResponse.btBuffer.Length);
                hlrResponse.OutputStream.Close();

                if (this.dgErrorMode() == false)
                {
                    hlHttp.BeginGetContext(this.ProcessRequest, hlHttp);
                }
                else
                {
                    this.dgNotifyError("슬라이드 쑈 제어 실패");
                }
            }
        }

        /*
        private NameValueCollection GetPOSTMethodContent(HttpListenerRequest hlrRequest)
        {
            NameValueCollection nvcResult;

            using (StreamReader srReader = new StreamReader(hlrRequest.InputStream, hlrRequest.ContentEncoding))
            {
                nvcResult = HttpUtility.ParseQueryString(srReader.ReadToEnd());
                srReader.Close();
                //srReader.Dispose();
            }

            return nvcResult;
        }
        */

        private Model.ResponseContent SelectResponseContent(HttpListenerRequest hlrRequest)
        {
            Model.ResponseContent rcResult = null;
            Model.SlideItem siSlide;
            StringBuilder sbResponseText;
            // 예 : http://127.0.0.1/MyTemp         --->    AbsolutePath : /MyTemp              ---> sRequestPagePath : empty
            // 예 : http://127.0.0.1/MyTemp/xxx.asp --->    AbsolutePath : /MyTemp/xxx.asp      ---> sRequestPagePath : /xxx.asp
            string sRequestPagePath = hlrRequest.Url.AbsolutePath.Substring((this.sURLRootDirectoryName.Length + 1)).ToUpper();
            bool bMatchAuthPassword = ((hlrRequest.QueryString["AuthPassword"] ?? String.Empty).Trim().ToUpper() == this.sConnectPassword_Upper);
            bool bMathWorkID = ((hlrRequest.QueryString["WorkID"] ?? String.Empty).Trim() == this.dgWorkID());
            string sProcessValue;
            int iCount;

            // 웹서비스 버젼 만들어야 함~~~~~~~~~~~!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // 오더 XML 제공용. 이 오더 XML에는 슬라이드 이미지 파일명(만!)이 있음!

            // 로그인 폼
            if (sRequestPagePath == String.Empty)
            {
                // TODO : [MEMO] HTML 디렉토리와 관계있음!
                sProcessValue = (Environment.CurrentDirectory + @"\HTML\Login.html");

                if ((sProcessValue != String.Empty) && (File.Exists(sProcessValue) == true))
                {
                    sbResponseText = new StringBuilder();
                    sbResponseText.AppendLine(File.ReadAllText(sProcessValue, Encoding.UTF8));
                    sbResponseText.Replace("@@URLRootDirectoryName@@", this.sURLRootDirectoryName);
                    sbResponseText.Replace("@@WrongPasswordBoxDisplay@@", "none");
                    sbResponseText.Replace("@@AssemblyName@@", XProvider.Value.AssemblyValue.sAssemblyName);
                    sbResponseText.Replace("@@AssemblyVersion@@", XProvider.Value.AssemblyValue.vAssemblyVersion.ToString());

                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sHTMLMimeType,
                        sbResponseText.ToString()
                    );
                }
            }
            // 로그인 체크
            else if (sRequestPagePath == "/LOGINOK")
            {
                sbResponseText = new StringBuilder();

                // 로그인 성공
                if (bMatchAuthPassword == true)
                {
                    // TODO : [MEMO] HTML 디렉토리와 관계있음!
                    sProcessValue = (Environment.CurrentDirectory + @"\HTML\LoginOK.html");

                    if ((sProcessValue != String.Empty) && (File.Exists(sProcessValue) == true))
                    {
                        sbResponseText.AppendLine(File.ReadAllText(sProcessValue, Encoding.UTF8));
                        sbResponseText.Replace("@@AuthPassword@@", this.sConnectPassword);
                        sbResponseText.Replace("@@URLRootDirectoryName@@", this.sURLRootDirectoryName);
                        sbResponseText.Replace("@@DocumentName@@", this.dgDocumentName());
                        sbResponseText.Replace("@@WorkID@@", this.dgWorkID());
                        sbResponseText.Replace("@@MaxSlideCount@@", this.dgSlideCount().ToString());
                        sbResponseText.Replace("@@SlideWidth@@", this.dgSlideSize().Width.ToString());
                        sbResponseText.Replace("@@SlideHeight@@", this.dgSlideSize().Height.ToString());
                        sbResponseText.Replace("@@AssemblyName@@", XProvider.Value.AssemblyValue.sAssemblyName);
                        sbResponseText.Replace("@@AssemblyVersion@@", XProvider.Value.AssemblyValue.vAssemblyVersion.ToString());

                        iCount = 1;
                        sbResponseText.Replace(
                            "@@SlideAreaItemList@@",
                            String.Join(
                                Environment.NewLine,
                                (
                                    (new string[this.dgSlideCount()])
                                        .Select(
                                            (
                                                (x) => (
                                                    (
                                                        @"
                        				                <li class=""SlideItem"">
                        					                <div class=""SlideImage""><img src=""/" + this.sURLRootDirectoryName + "/GetSlide?AuthPassword=" + this.sConnectPassword + "&WorkID=" + this.dgWorkID() + "&No=" + iCount + @""" /></div>
                        					                <div class=""SlideNote"">" + this.dgSlideItem((iCount++).ToString()).sMemo.Replace("\r", ("<br />" + Environment.NewLine)) + @"</div>
                        				                </li>
                                                    "
                                                    )
                                                )
                                            )
                                        )
                                )
                            )
                        );

                        iCount = 1;
                        sbResponseText.Replace(
                            "@@SlideListAreaItemList@@",
                            String.Join(
                                Environment.NewLine,
                                (
                                    (new string[this.dgSlideCount()])
                                        .Select(
                                            (
                                                (x) => (
                                                    (
                                                        @"<li class=""SlideItem""><img src=""/" + this.sURLRootDirectoryName + "/GetSlide?AuthPassword=" + this.sConnectPassword + "&WorkID=" + this.dgWorkID() + "&No=" + (iCount++) + @""" /></li>"
                                                    )
                                                )
                                            )
                                        )
                                )
                            )
                        );
                    }
                }
                // 로그인 실패
                else
                {
                    // TODO : [MEMO] HTML 디렉토리와 관계있음!
                    sProcessValue = (Environment.CurrentDirectory + @"\HTML\Login.html");

                    if ((sProcessValue != String.Empty) && (File.Exists(sProcessValue) == true))
                    {
                        sbResponseText.AppendLine(File.ReadAllText(sProcessValue, Encoding.UTF8));
                        sbResponseText.Replace("@@URLRootDirectoryName@@", this.sURLRootDirectoryName);
                        sbResponseText.Replace("@@WrongPasswordBoxDisplay@@", "block");
                        sbResponseText.Replace("@@AssemblyName@@", XProvider.Value.AssemblyValue.sAssemblyName);
                        sbResponseText.Replace("@@AssemblyVersion@@", XProvider.Value.AssemblyValue.vAssemblyVersion.ToString());
                    }
                }

                if (sbResponseText.Length > 0)
                {
                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sHTMLMimeType,
                        sbResponseText.ToString()
                    );
                }
            }
            // 슬라이드 이미지 다운로드
            else if ((sRequestPagePath == "/GETSLIDE") && ((bMatchAuthPassword == true) && (bMathWorkID == true)))
            {
                siSlide = this.dgSlideItem((hlrRequest.QueryString["No"] ?? "0").Trim());

                if(siSlide != null) {
                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sPNGMimeType,
                        File.ReadAllBytes(siSlide.sExportFilePath)
                    );
                }
            }
            // 자바스크립트 다운로드
            // TODO : [MEMO] JAS 디렉토리와 관계있음!
            // 수동으로 이래 단순 무식하게 한 이유는..... 보안 문제로 지정된 파일 외 다른 파일 다운로드 금지!
            else if ((sRequestPagePath == "/JAS/JQUERY-1.9.1.MIN.JS") || (sRequestPagePath == "/JAS/JQUERY.CYCLE.ALL_EDIT.JS") || (sRequestPagePath == "/JAS/JQUERY.TOUCHWIPE.JS"))
            {
                if (sRequestPagePath == "/JAS/JQUERY-1.9.1.MIN.JS")
                {
                    sProcessValue = (Environment.CurrentDirectory + @"\JAS\jquery-1.9.1.min.js");
                }
                else if (sRequestPagePath == "/JAS/JQUERY.CYCLE.ALL_EDIT.JS")
                {
                    sProcessValue = (Environment.CurrentDirectory + @"\JAS\jquery.cycle.all_edit.js");
                }
                else if (sRequestPagePath == "/JAS/JQUERY.TOUCHWIPE.JS")
                {
                    sProcessValue = (Environment.CurrentDirectory + @"\JAS\jquery.touchwipe.js");
                }
                else
                {
                    sProcessValue = String.Empty;
                }

                if ((sProcessValue != String.Empty) && (File.Exists(sProcessValue) == true))
                {
                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sJavaScriptMimeType,
                        File.ReadAllBytes(sProcessValue)
                    );
                }
            }
            // 스타일시트 다운로드
            // TODO : [MEMO] CSS 디렉토리와 관계있음!
            else if (sRequestPagePath == "/CSS/DEFAULTSTYLESHEET.CSS")
            {
                sProcessValue = (Environment.CurrentDirectory + @"\CSS\DefaultStyleSheet.css");

                if ((sProcessValue != String.Empty) && (File.Exists(sProcessValue) == true))
                {
                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sStyleSheetMimeType,
                        File.ReadAllBytes(sProcessValue)
                    );
                }
            }
            // 슬라이드 쑈 컨트롤
            else if ((sRequestPagePath == "/COMMAND") && ((bMatchAuthPassword == true) && (bMathWorkID == true)))
            {
                sProcessValue = (hlrRequest.QueryString["CommandType"] ?? String.Empty).Trim().ToUpper();

                if (
                    (sProcessValue != String.Empty) &&
                    (Regex.IsMatch(sProcessValue, "^RUN|FIRST|PREVIOUS|NEXT|LAST|MOVE,[0-9]+|END$", RegexOptions.IgnoreCase) == true) &&
                    (this.dgSlideShowCommand(sProcessValue.Split(new string[] { "," }, StringSplitOptions.None)) == true)
                )
                {
                    sbResponseText = new StringBuilder();
                    sbResponseText.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
                    sbResponseText.AppendLine("<command>");
                    sbResponseText.AppendLine(" <statusCode>OK</statusCode>");
                    sbResponseText.AppendLine("</command>");

                    rcResult = new Model.ResponseContent(
                        Value.NetworkValue.iHTTPStatusCode_OK,
                        Value.MimeTypeValue.sXMLMimeType,
                        sbResponseText.ToString()
                    );
                }
            }

            // 아무 결과도 없으면 404 오류!
            if (rcResult == null)
            {
                rcResult = new Model.ResponseContent(
                    Value.NetworkValue.iHTTPStatusCode_NotFound, 
                    Value.MimeTypeValue.sHTMLMimeType,
                    (
                        @"
                            <!DOCTYPE html>
                            <html lang=""ko"">
	                            <head>
		                            <meta charset=""utf-8"">
		                            <title> 404 - Not Found </title>
		                            <meta name=""viewport"" content=""width=320, user-scalable=1; target-densitydpi=medium-dpi"" />
	                            </head>
	                            <body>
		                            <h1>404 - Not Found</h1>
		                            <h3>해당 경로에 파일이 없습니다.</h3>
	                            </body>
                            </html>
                        "
                    )
                );
            }

            return rcResult;
        }

        // ------------------------------------------

        public SingleServer(
            int iPort, 
            string sURLRootDirectoryName,
            WorkDelegate.dgGetWorkID dgWorkID,
            WorkDelegate.dgGetSlideItem dgSlideItem,
            WorkDelegate.dgGetDocumentName dgDocumentName,
            WorkDelegate.dgGetSlideSize dgSlideSize,
            WorkDelegate.dgGetTotalSlideCount dgSlideCount,
            WorkDelegate.dgSetSlideShowCommand dgSlideShowCommand,
            WorkDelegate.dgGetNowErrorMode dgErrorMode,
            WorkDelegate.dgSetNotifyErrorMode dgNotifyError
        )
        {
            this.iPort = iPort;
            this.sURLRootDirectoryName = sURLRootDirectoryName;
            this.dgWorkID = dgWorkID;
            this.dgSlideItem = dgSlideItem;
            this.dgDocumentName = dgDocumentName;
            this.dgSlideSize = dgSlideSize;
            this.dgSlideCount = dgSlideCount;
            this.dgSlideShowCommand = dgSlideShowCommand;
            this.dgErrorMode = dgErrorMode;
            this.dgNotifyError = dgNotifyError;

            this.sConnectPassword = String.Empty;
            this.sConnectPassword_Upper = String.Empty;
            
            this.hlHttp = new HttpListener();
            // + : Any IP
            this.hlHttp.Prefixes.Add(("http://+:" + iPort + "/" + sURLRootDirectoryName + "/"));
            this.hlHttp.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }

        public void StartServer(string sConnectPassword)
        {
            if (this.hlHttp.IsListening == false)
            {
                this.sConnectPassword = sConnectPassword;
                this.sConnectPassword_Upper = sConnectPassword.ToUpper();

                this.hlHttp.Start();
                this.hlHttp.BeginGetContext(this.ProcessRequest, this.hlHttp);
            }
        }

        public void StopServer()
        {
            if (this.hlHttp.IsListening == true)
            {
                this.hlHttp.Stop();
                this.hlHttp.Abort();
                this.hlHttp.Close();
            }
        }
    }
}
