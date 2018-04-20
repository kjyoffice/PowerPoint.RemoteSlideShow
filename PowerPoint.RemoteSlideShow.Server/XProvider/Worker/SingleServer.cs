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
        private Func<string> WorkID { get; set; }
        private Func<string, Model.SlideItem> SlideItem { get; set; }
        private Func<string> DocumentName { get; set; }
        private Func<Size> SlideSize { get; set; }
        private Func<int> SlideCount { get; set; }
        private Func<string[], bool> SlideShowCommand { get; set; }
        private Func<bool> ErrorMode { get; set; }
        private Action<string> NotifyError { get; set; }
        private HttpListener HttpWorker { get; set; }
        private int HttpPortNo { get; set; }
        private string URLRootDirectoryName { get; set; }
        public string ConnectPassword { get; private set; }
        private string ConnectPasswordUpper { get; set; }

        // ------------------------------------------

        public string[] ConnectURL
        {
            get
            {
                string baseURL = ("http://{0}" + ((this.HttpPortNo != 80) ? (":" + this.HttpPortNo) : String.Empty) + "/" + this.URLRootDirectoryName + "/");
                string[] result = Value.NetworkValue.LANIPAddress.Select(x => String.Format(baseURL, x.ToString())).ToArray();

                return result;
            }
        }

        // ------------------------------------------

        private void ProcessRequest(IAsyncResult iar)
        {
            HttpListener hl = (iar.AsyncState as HttpListener);

            if ((hl != null) && (hl.IsListening == true))
            {
                HttpListenerContext hlc = hl.EndGetContext(iar);

                Model.ResponseContent rc = this.SelectResponseContent(hlc.Request);

                using (HttpListenerResponse hlRes = hlc.Response)
                {
                    hlRes.StatusCode = rc.StatusCode;
                    hlRes.StatusDescription = rc.StatusDescription;
                    hlRes.ContentType = rc.ContentType;
                    hlRes.OutputStream.Write(rc.OutputBuffer, 0, rc.OutputBuffer.Length);
                    hlRes.OutputStream.Close();
                }

                if (this.ErrorMode() == false)
                {
                    hl.BeginGetContext(this.ProcessRequest, hl);
                }
                else
                {
                    this.NotifyError("슬라이드 쑈 제어 실패");
                }
            }
        }

        private NameValueCollection GetPOSTMethodContent(HttpListenerRequest hlReq)
        {
            NameValueCollection result;

            using (StreamReader sr = new StreamReader(hlReq.InputStream, hlReq.ContentEncoding))
            {
                result = HttpUtility.ParseQueryString(sr.ReadToEnd());
                sr.Close();
            }

            return result;
        }

        private Model.ResponseContent SelectResponseContent_Login(bool isWrongPassword)
        {
            // TODO : [MEMO] HTML 디렉토리와 관계있음!
            string processValue = (Environment.CurrentDirectory + @"\HTML\Login.html");
            Model.ResponseContent result = null;

            if (File.Exists(processValue) == true)
            {
                StringBuilder responseText = new StringBuilder();
                responseText.AppendLine(File.ReadAllText(processValue, Encoding.UTF8));
                responseText.Replace("@@URLRootDirectoryName@@", this.URLRootDirectoryName);
                responseText.Replace("@@DocumentName@@", this.DocumentName());
                responseText.Replace("@@WrongPasswordBoxDisplay@@", ((isWrongPassword == true) ? "block" : "none"));
                responseText.Replace("@@AssemblyName@@", XProvider.Value.AssemblyValue.Name);
                responseText.Replace("@@AssemblyVersion@@", XProvider.Value.AssemblyValue.Version.ToString());

                result = new Model.ResponseContent(
                    Value.NetworkValue.HTTPOK,
                    Value.MimeTypeValue.HTML,
                    responseText.ToString()
                );
            }

            return result;
        }

        private Model.ResponseContent SelectResponseContent_LoginOK()
        {
            // TODO : [MEMO] HTML 디렉토리와 관계있음!
            string processValue = (Environment.CurrentDirectory + @"\HTML\LoginOK.html");
            Model.ResponseContent result = null;

            if (File.Exists(processValue) == true)
            {
                StringBuilder responseText = new StringBuilder();
                responseText.AppendLine(File.ReadAllText(processValue, Encoding.UTF8));
                responseText.Replace("@@AuthPassword@@", this.ConnectPassword);
                responseText.Replace("@@URLRootDirectoryName@@", this.URLRootDirectoryName);
                responseText.Replace("@@DocumentName@@", this.DocumentName());
                responseText.Replace("@@WorkID@@", this.WorkID());
                responseText.Replace("@@MaxSlideCount@@", this.SlideCount().ToString());
                responseText.Replace("@@SlideWidth@@", this.SlideSize().Width.ToString());
                responseText.Replace("@@SlideHeight@@", this.SlideSize().Height.ToString());
                responseText.Replace("@@AssemblyName@@", XProvider.Value.AssemblyValue.Name);
                responseText.Replace("@@AssemblyVersion@@", XProvider.Value.AssemblyValue.Version.ToString());

                int count1 = 1;
                responseText.Replace(
                    "@@SlideAreaItemList@@",
                    String.Join(
                        Environment.NewLine,
                        (
                            (new string[this.SlideCount()])
                                .Select(
                                    (
                                        (x) => (
                                            (
                                                @"
                        				                <li class=""" + ((count1 <= 1) ? "boxblock" : "boxnone") + @""">
                        					                <img src=""/" + this.URLRootDirectoryName + "/GetSlide?authpassword=" + this.ConnectPassword + "&workid=" + this.WorkID() + "&no=" + count1 + @""" />
                        					                <div>" + this.SlideItem((count1++).ToString()).Memo.Replace("\r", ("<br />" + Environment.NewLine)) + @"</div>
                        				                </li>
                                                    "
                                            )
                                        )
                                    )
                                )
                        )
                    )
                );

                int count2 = 1;
                responseText.Replace(
                    "@@SlideListAreaItemList@@",
                    String.Join(
                        Environment.NewLine,
                        (
                            (new string[this.SlideCount()])
                                .Select(
                                    (
                                        (x) => (
                                            (
                                                @"<li class=""" + ((count2 <= 1) ? "on" : "off") + @""" onclick=""SlideShowCommand('MOVE," + count2 + @"');""><img src=""/" + this.URLRootDirectoryName + "/GetSlide?authpassword=" + this.ConnectPassword + "&workid=" + this.WorkID() + "&no=" + (count2++) + @""" /></li>"
                                            )
                                        )
                                    )
                                )
                        )
                    )
                );

                result = new Model.ResponseContent(
                    Value.NetworkValue.HTTPOK,
                    Value.MimeTypeValue.HTML,
                    responseText.ToString()
                );
            }

            return result;
        }

        private Model.ResponseContent SelectResponseContent_DefaultStyle()
        {
            string processValue = (Environment.CurrentDirectory + @"\CSS\DefaultStyle.css");
            Model.ResponseContent result = null;

            if (File.Exists(processValue) == true)
            {
                result = new Model.ResponseContent(
                    Value.NetworkValue.HTTPOK,
                    Value.MimeTypeValue.StyleSheet,
                    File.ReadAllBytes(processValue)
                );
            }

            return result;
        }

        private Model.ResponseContent SelectResponseContent_404NotFound()
        {
            return new Model.ResponseContent(
                Value.NetworkValue.HTTPNotFound,
                Value.MimeTypeValue.HTML,
                (
                    @"
                        <!DOCTYPE html>
                        <html lang=""ko"">
	                        <head>
		                        <meta charset=""utf-8"">
		                        <title> 404 - Not Found </title>
		                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0, target-densitydpi=medium-dpi"" />
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

        private Model.ResponseContent SelectResponseContent(HttpListenerRequest hlReq)
        {
            Model.ResponseContent result = null;
            //// 예 : http://192.168.0.1/MyTemp         --->    AbsolutePath : /MyTemp              ---> requestPagePath : empty
            //// 예 : http://192.168.0.1/MyTemp/xxx.asp --->    AbsolutePath : /MyTemp/xxx.asp      ---> requestPagePath : /xxx.asp
            string requestPagePath = hlReq.Url.AbsolutePath.Substring((this.URLRootDirectoryName.Length + 1)).ToUpper();
            bool matchAuthPassword = ((hlReq.QueryString["AuthPassword"] ?? String.Empty).Trim().ToUpper() == this.ConnectPasswordUpper);
            bool mathWorkID = ((hlReq.QueryString["WorkID"] ?? String.Empty).Trim() == this.WorkID());

            // 로그인 폼
            if (requestPagePath == String.Empty)
            {
                result = this.SelectResponseContent_Login(false);
            }
            // 로그인 체크
            else if (requestPagePath == "/LOGINOK")
            {
                // 로그인 성공
                if (matchAuthPassword == true)
                {
                    result = this.SelectResponseContent_LoginOK();
                }
                // 로그인 실패
                else
                {
                    result = this.SelectResponseContent_Login(true);
                }
            }
            else if (requestPagePath == "/CSS/DEFAULTSTYLE.CSS")
            {
                result = this.SelectResponseContent_DefaultStyle();
            }
            // 슬라이드 이미지 다운로드
            else if ((requestPagePath == "/GETSLIDE") && ((matchAuthPassword == true) && (mathWorkID == true)))
            {
                Model.SlideItem slide = this.SlideItem((hlReq.QueryString["No"] ?? "0").Trim());

                if(slide != null) {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.PNG,
                        File.ReadAllBytes(slide.ExportFilePath)
                    );
                }
            }
            // 슬라이드 쑈 컨트롤
            else if ((requestPagePath == "/COMMAND") && ((matchAuthPassword == true) && (mathWorkID == true)))
            {
                string processValue = (hlReq.QueryString["CommandType"] ?? String.Empty).Trim().ToUpper();

                if (
                    (processValue != String.Empty) &&
                    (Regex.IsMatch(processValue, "^RUN|FIRST|PREVIOUS|NEXT|LAST|MOVE,[0-9]+|END$", RegexOptions.IgnoreCase) == true) &&
                    (this.SlideShowCommand(processValue.Split(new string[] { "," }, StringSplitOptions.None)) == true)
                )
                {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.JSON,
                        "{ \"statusCode\" : \"OK\" }"
                    );
                }
            }

            // 아무 결과도 없으면 404 오류!
            if (result == null)
            {
                result = this.SelectResponseContent_404NotFound();
            }

            return result;
        }

        // ------------------------------------------

        public SingleServer(
            int httpPortNo, 
            string urlRootDirectoryName,
            Func<string> workID,
            Func<string, Model.SlideItem> slideItem,
            Func<string> documentName,
            Func<Size> slideSize,
            Func<int> slideCount,
            Func<string[], bool> slideShowCommand,
            Func<bool> errorMode,
            Action<string> notifyError
        )
        {
            this.HttpPortNo = httpPortNo;
            this.URLRootDirectoryName = urlRootDirectoryName;
            this.WorkID = workID;
            this.SlideItem = slideItem;
            this.DocumentName = documentName;
            this.SlideSize = slideSize;
            this.SlideCount = slideCount;
            this.SlideShowCommand = slideShowCommand;
            //this.SlideShowCommand = delegate(string[] x) { return true; };
            this.ErrorMode = errorMode;
            this.NotifyError = notifyError;

            this.ConnectPassword = String.Empty;
            this.ConnectPasswordUpper = String.Empty;
            
            this.HttpWorker = new HttpListener();
            // + : Any IP
            this.HttpWorker.Prefixes.Add(("http://+:" + httpPortNo + "/" + urlRootDirectoryName + "/"));
            this.HttpWorker.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }

        public void StartServer(string connectPassword)
        {
            if (this.HttpWorker.IsListening == false)
            {
                this.ConnectPassword = connectPassword;
                this.ConnectPasswordUpper = connectPassword.ToUpper();

                this.HttpWorker.Start();
                this.HttpWorker.BeginGetContext(this.ProcessRequest, this.HttpWorker);
            }
        }

        public void StopServer()
        {
            if (this.HttpWorker.IsListening == true)
            {
                this.HttpWorker.Stop();
                this.HttpWorker.Abort();
                this.HttpWorker.Close();
            }
        }
    }
}
