# PowerPoint.RemoteSlideShow
파워포인트 슬라이드 쑈를 위해 핸드폰을 원격 기기로 사용할 수 있습니다.<br>
Remote control for PowerPoint slideshow 

## 빌드 (Build)
VisualStudio 이용해 솔루션 파일, "PowerPoint.RemoteSlideShow.sln"을 열어 빌드합니다.<br>
Open solution file "PowerPoint.RemoteSlideShow.sln" on VisualStudio and Build.

## 실행 (Run)
빌드 된 "PowerPoint.RemoteSlideShow.Server.exe"(이하 프로그램) 및 PowerPoint를 관리자 권한으로 실행합니다.<br>
이후 슬라이드 쑈를 하고자 하는 PPT/PPTX 문서를 열고 프로그램의 "새로고침"버튼을 눌러 해당 문서를 선택, "파워포인트 원격 슬라이드 쑈" 버튼을 누릅니다.

Build file "PowerPoint.RemoteSlideShow.Server.exe"(as Program) and PowerPoint run as Administrator account.<br>
Open slideshow document(PPT/PPTX), select executing "Program".
Press "새로고침"(Refresh) button, select document after press "파워포인트 원격 슬라이드 쑈"(PowerPoint remote slideshow) button.

## 접속 (Connect)
프로그램이 실행중인 PC와 같은 네트워크에 원격 기기로 쓸 핸드폰을 등록합니다.

* 보통 핸드폰의 WiFi를 활성화 합니다.

Register remote device, execut program PC.

* Remote device on WiFi


이후 핸드폰의 웹 브라우저 앱을 열어 프로그램에서 안내된 경로로 접속합니다.<br>
로그인 페이지가 열리면 프로그램에 안내된 비밀번호를 입력하여 프로그램과 핸드폰을 연결합니다.

Execute Remote device web browser, try connect "Program" notify URL.<br>
Open login page, type password "Program" notify password.

* * *

## 안내 (Information)
1. 프로그램 및 PowerPoint를 관리자 권한으로 열지 않도록 하기 위해서는 다음과 같은 설정이 필요합니다. 
    1. 시작 -> 실행 -> cmd
    2. netsh http add urlacl url=http://+:80/SlideShow/ user=[Application 실행 계정] 

    * "Application 실행 계정"을 알기 위해서는 다음의 명령을 실행하면 됩니다.<br>
        echo %username%
    
    * 만약 2번 항목에서 등록한 항목을 삭제하고자 한다면 다음 명령을 실행하면 됩니다.<br>
        netsh http delete urlacl url=http://+:80/SlideShow/ 

    * 여기서 지정된 "80"포트 및 "SlideShow"는 app.config(/PowerPoint.RemoteSlideShow.Server.exe.config)의 다음 설정에 따릅니다.<br>
        SingleServerPortNo<br>
        SingleServerRootDirectoryName

1. Not use run as Administrator, try this setting.
    1. start -> run -> cmd
    2. netsh http add urlacl url=http://+:80/SlideShow/ user=[Application execute account] 

    * If you don't know "Application execute account", try this console command.<br>
        ehco %username%

    * Delete regist setting by no.2<br>
        netsh http delete urlacl url=http://+:80/SlideShow/ 

    * Url port "80" and virtual directory name "SlideShow" is typed app.config(/PowerPoint.RemoteSlideShow.Server.exe.config).
        SingleServerPortNo<br>
        SingleServerRootDirectoryName    

2. 핸드폰에서 서버 연결이 안되는 경우 프로그램을 실행하는 PC의 방화벽에서 80포트를 열어야 합니다

    * 제어판 -> 관리도구 -> 고급 보안이 포함된 Windows 방화벽<br>
      OR<br>
      시작 -> 실행 -> wf.msc

    * 여기서 지정된 "80"포트는 app.config(/PowerPoint.RemoteSlideShow.Server.exe.config)의 다음 설정에 따릅니다.<br>
        SingleServerPortNo

2. Not connect remote control server, please check PC firewall and open 80 port.    

    * Control Panel -> Administrative tools -> Windows Firewall with Advanced Security<br>
      OR<br>
      start -> run -> wf.msc

    * Url port "80" is typed app.config(/PowerPoint.RemoteSlideShow.Server.exe.config).<br>	
        SingleServerPortNo     

## app.config(/PowerPoint.RemoteSlideShow.Server.exe.config)
1. SlideExportDirectoryPath

    원격 기기에서 보여질 슬라이드 이미지가 저장되는 디렉토리 경로입니다.<br>
    이 디렉토리 경로가 빈 값인 경우 "PowerPoint.RemoteSlideShow.Server.exe"파일이 저장된 위치에 슬라이드 이미지가 저장됩니다.

    Save directory path for remote device show slide image.<br>
    If this option is empty value, use "PowerPoint.RemoteSlideShow.Server.exe" file save directory

2. SingleServerPortNo

    웹서버 포트번호 입니다.<br>
    Webserver port no.

3. SingleServerRootDirectoryName

    웹페이지 가상 디렉토리 이름입니다.<br>
    WebPage virtual directory name.

## 확인된/의존성 PowerPoint (Check/Dependency PowerPoint)
2007, 2010, 365

* * *

## 도움 (Help)
이 프로그램을 공부삼아 만들었으며 다음 포스팅을 참고하였습니다.<br>
This program is for study and reference posting.

[C# - 오피스 파워포인트(Powerpoint) 파일을 WinForm에서 보는 방법][sysnet1]<br>
[http://www.sysnet.pe.kr/2/0/1728][sysnet1]

[IIS의 80 포트를 공유하는 응용 프로그램 만드는 방법][sysnet2]<br>
[http://www.sysnet.pe.kr/2/0/1555][sysnet2]

[sysnet1]: http://www.sysnet.pe.kr/2/0/1728
[sysnet2]: http://www.sysnet.pe.kr/2/0/1555

* * *

## 알려진 문제점 (Known problem)

1. UI 구림<br />
    UI is shit

2. 만든애가 영어가 너무 짧아서 텍스트가 엉망진창임<br />
    KimJaeYoung is very very little bit english. terrible text

3. 공개된 연결 비밀번호. 낮은 보안성<br />
    Opend connect password. low security

* * *

## 수정내역 (Fix log)

##### 2017-10-10
관리자 권한 요청 매니페스트 파일 추가<br>
Add manifest file for require administrator level.

[비주얼 스튜디오에서 관리자 권한을 요구하는 C# 콘솔 프로그램 제작][sysnet3]<br>
[http://www.sysnet.pe.kr/2/0/11318][sysnet3]

[sysnet3]: http://www.sysnet.pe.kr/2/0/11318

##### 2018-04-20
"이 프로그램은" 버튼 삭제<br />
Deleted button to "이 프로그램은"

연결 가능한 URL 모두가 표시되도록 함<br />
All display connect ready URL<br />
+<br />
UI 일부 수정<br />
UI Modify


