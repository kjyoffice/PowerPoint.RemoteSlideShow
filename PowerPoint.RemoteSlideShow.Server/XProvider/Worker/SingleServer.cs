﻿using System;
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
        private WorkDelegate.GetWorkID WorkID { get; set; }
        private WorkDelegate.GetSlideItem SlideItem { get; set; }
        private WorkDelegate.GetDocumentName DocumentName { get; set; }
        private WorkDelegate.GetSlideSize SlideSize { get; set; }
        private WorkDelegate.GetTotalSlideCount SlideCount { get; set; }
        private WorkDelegate.SetSlideShowCommand SlideShowCommand { get; set; }
        private WorkDelegate.GetNowErrorMode ErrorMode { get; set; }
        private WorkDelegate.SetNotifyErrorMode NotifyError { get; set; }
        private HttpListener HttpWorker { get; set; }
        private int HttpPortNo { get; set; }
        private string URLRootDirectoryName { get; set; }
        public string ConnectPassword { get; private set; }
        private string ConnectPasswordUpper { get; set; }

        // ------------------------------------------

        public string ConnectURL
        {
            get
            {
                return (
                    "http://" + 
                    (
                        Value.NetworkValue.LANIPAddress + 
                        ((this.HttpPortNo != 80) ? (":" + this.HttpPortNo) : String.Empty)
                    ) + 
                    "/" + this.URLRootDirectoryName + "/"
                );
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

                HttpListenerResponse hlRes = hlc.Response;
                hlRes.StatusCode = rc.StatusCode;
                hlRes.StatusDescription = rc.StatusDescription;
                hlRes.ContentType = rc.ContentType;
                hlRes.OutputStream.Write(rc.OutputBuffer, 0, rc.OutputBuffer.Length);
                hlRes.OutputStream.Close();
                hlRes.OutputStream.Dispose();

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

        private Model.ResponseContent SelectResponseContent(HttpListenerRequest hlRes)
        {
            Model.ResponseContent result = null;
            //// 예 : http://127.0.0.1/MyTemp         --->    AbsolutePath : /MyTemp              ---> requestPagePath : empty
            //// 예 : http://127.0.0.1/MyTemp/xxx.asp --->    AbsolutePath : /MyTemp/xxx.asp      ---> requestPagePath : /xxx.asp
            string requestPagePath = hlRes.Url.AbsolutePath.Substring((this.URLRootDirectoryName.Length + 1)).ToUpper();
            bool matchAuthPassword = ((hlRes.QueryString["AuthPassword"] ?? String.Empty).Trim().ToUpper() == this.ConnectPasswordUpper);
            bool mathWorkID = ((hlRes.QueryString["WorkID"] ?? String.Empty).Trim() == this.WorkID());
            string assemblyVersion = XProvider.Value.AssemblyValue.Version.ToString();
            string assemblyName = XProvider.Value.AssemblyValue.Name;

            // 로그인 폼
            if (requestPagePath == String.Empty)
            {
                // TODO : [MEMO] HTML 디렉토리와 관계있음!
                string processValue = (Environment.CurrentDirectory + @"\HTML\Login.html");

                if ((processValue != String.Empty) && (File.Exists(processValue) == true))
                {
                    StringBuilder responseText = new StringBuilder();
                    responseText.AppendLine(File.ReadAllText(processValue, Encoding.UTF8));
                    responseText.Replace("@@URLRootDirectoryName@@", this.URLRootDirectoryName);
                    responseText.Replace("@@WrongPasswordBoxDisplay@@", "none");
                    responseText.Replace("@@AssemblyName@@", assemblyName);
                    responseText.Replace("@@AssemblyVersion@@", assemblyVersion);

                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.HTML,
                        responseText.ToString()
                    );
                }
            }
            // 로그인 체크
            else if (requestPagePath == "/LOGINOK")
            {
                StringBuilder responseText = new StringBuilder();

                // 로그인 성공
                if (matchAuthPassword == true)
                {
                    // TODO : [MEMO] HTML 디렉토리와 관계있음!
                    string processValue = (Environment.CurrentDirectory + @"\HTML\LoginOK.html");

                    if ((processValue != String.Empty) && (File.Exists(processValue) == true))
                    {
                        responseText.AppendLine(File.ReadAllText(processValue, Encoding.UTF8));
                        responseText.Replace("@@AuthPassword@@", this.ConnectPassword);
                        responseText.Replace("@@URLRootDirectoryName@@", this.URLRootDirectoryName);
                        responseText.Replace("@@DocumentName@@", this.DocumentName());
                        responseText.Replace("@@WorkID@@", this.WorkID());
                        responseText.Replace("@@MaxSlideCount@@", this.SlideCount().ToString());
                        responseText.Replace("@@SlideWidth@@", this.SlideSize().Width.ToString());
                        responseText.Replace("@@SlideHeight@@", this.SlideSize().Height.ToString());
                        responseText.Replace("@@AssemblyName@@", assemblyName);
                        responseText.Replace("@@AssemblyVersion@@", assemblyVersion);

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
                        				                <li class=""SlideItem"">
                        					                <div class=""SlideImage""><img src=""/" + this.URLRootDirectoryName + "/GetSlide?AuthPassword=" + this.ConnectPassword + "&WorkID=" + this.WorkID() + "&No=" + count1 + @""" /></div>
                        					                <div class=""SlideNote"">" + this.SlideItem((count1++).ToString()).Memo.Replace("\r", ("<br />" + Environment.NewLine)) + @"</div>
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
                                                        @"<li class=""SlideItem""><img src=""/" + this.URLRootDirectoryName + "/GetSlide?AuthPassword=" + this.ConnectPassword + "&WorkID=" + this.WorkID() + "&No=" + (count2++) + @""" /></li>"
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
                    string processValue = (Environment.CurrentDirectory + @"\HTML\Login.html");

                    if ((processValue != String.Empty) && (File.Exists(processValue) == true))
                    {
                        responseText.AppendLine(File.ReadAllText(processValue, Encoding.UTF8));
                        responseText.Replace("@@URLRootDirectoryName@@", this.URLRootDirectoryName);
                        responseText.Replace("@@WrongPasswordBoxDisplay@@", "block");
                        responseText.Replace("@@AssemblyName@@", assemblyName);
                        responseText.Replace("@@AssemblyVersion@@", assemblyVersion);
                    }
                }

                if (responseText.Length > 0)
                {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.HTML,
                        responseText.ToString()
                    );
                }
            }
            // 슬라이드 이미지 다운로드
            else if ((requestPagePath == "/GETSLIDE") && ((matchAuthPassword == true) && (mathWorkID == true)))
            {
                Model.SlideItem slide = this.SlideItem((hlRes.QueryString["No"] ?? "0").Trim());

                if(slide != null) {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.PNG,
                        File.ReadAllBytes(slide.ExportFilePath)
                    );
                }
            }
            // 자바스크립트 다운로드
            // TODO : [MEMO] JS 디렉토리와 관계있음!
            // 수동으로 이래 단순 무식하게 한 이유는..... 보안 문제로 지정된 파일 외 다른 파일 다운로드 금지!
            else if ((requestPagePath == "/JS/JQUERY-1.9.1.MIN.JS") || (requestPagePath == "/JS/JQUERY.CYCLE.ALL_EDIT.JS") || (requestPagePath == "/JS/JQUERY.TOUCHWIPE.JS"))
            {
                string processValue;

                if (requestPagePath == "/JS/JQUERY-1.9.1.MIN.JS")
                {
                    processValue = (Environment.CurrentDirectory + @"\JS\jquery-1.9.1.min.js");
                }
                else if (requestPagePath == "/JS/JQUERY.CYCLE.ALL_EDIT.JS")
                {
                    processValue = (Environment.CurrentDirectory + @"\JS\jquery.cycle.all_edit.js");
                }
                else if (requestPagePath == "/JS/JQUERY.TOUCHWIPE.JS")
                {
                    processValue = (Environment.CurrentDirectory + @"\JS\jquery.touchwipe.js");
                }
                else
                {
                    processValue = String.Empty;
                }

                if ((processValue != String.Empty) && (File.Exists(processValue) == true))
                {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.JavaScript,
                        File.ReadAllBytes(processValue)
                    );
                }
            }
            // 스타일시트 다운로드
            // TODO : [MEMO] CSS 디렉토리와 관계있음!
            else if (requestPagePath == "/CSS/DEFAULTSTYLESHEET.CSS")
            {
                string processValue = (Environment.CurrentDirectory + @"\CSS\DefaultStyleSheet.css");

                if ((processValue != String.Empty) && (File.Exists(processValue) == true))
                {
                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.StyleSheet,
                        File.ReadAllBytes(processValue)
                    );
                }
            }
            // 슬라이드 쑈 컨트롤
            else if ((requestPagePath == "/COMMAND") && ((matchAuthPassword == true) && (mathWorkID == true)))
            {
                string processValue = (hlRes.QueryString["CommandType"] ?? String.Empty).Trim().ToUpper();

                if (
                    (processValue != String.Empty) &&
                    (Regex.IsMatch(processValue, "^RUN|FIRST|PREVIOUS|NEXT|LAST|MOVE,[0-9]+|END$", RegexOptions.IgnoreCase) == true) &&
                    (this.SlideShowCommand(processValue.Split(new string[] { "," }, StringSplitOptions.None)) == true)
                )
                {
                    StringBuilder responseText = new StringBuilder();
                    responseText.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
                    responseText.AppendLine("<command>");
                    responseText.AppendLine(" <statusCode>OK</statusCode>");
                    responseText.AppendLine("</command>");

                    result = new Model.ResponseContent(
                        Value.NetworkValue.HTTPOK,
                        Value.MimeTypeValue.XML,
                        responseText.ToString()
                    );
                }
            }

            // 아무 결과도 없으면 404 오류!
            if (result == null)
            {
                result = new Model.ResponseContent(
                    Value.NetworkValue.HTTPNotFound, 
                    Value.MimeTypeValue.HTML,
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

            return result;
        }

        // ------------------------------------------

        public SingleServer(
            int httpPortNo, 
            string urlRootDirectoryName,
            WorkDelegate.GetWorkID workID,
            WorkDelegate.GetSlideItem slideItem,
            WorkDelegate.GetDocumentName documentName,
            WorkDelegate.GetSlideSize slideSize,
            WorkDelegate.GetTotalSlideCount slideCount,
            WorkDelegate.SetSlideShowCommand slideShowCommand,
            WorkDelegate.GetNowErrorMode errorMode,
            WorkDelegate.SetNotifyErrorMode notifyError
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