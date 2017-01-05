# PowerPoint.RemoteSlideShow
파워포인트 원격 슬라이드 쑈

같은 네트워크에 속한 기기간 슬라이드 쑈를 제어할 수 있습니다.

<<< 필요 항목 >>>
Microsoft Office PowerPoint 2007, 2010
Microsoft Office365 PowerPoint

<< SingleServer >>
슬라이드 쑈를 컨트롤 하기 위한 프로그램 내부 웹 서버로써 위 2가지 설정에 따른 동작을 하게 됨
즉 포트번호가 80이고 루트 디렉토리 이름이 SlideShow인 경우 접속 주소는 아래와 같다.
http://IP/SlideShow
(** 만약 포트번호가 80이 아닌 다른 번호인 경우 "IP:PORT"가 됨)    

이 서버로 인해 Vista부터는 UAC에 영향을 받게 됨
그로인해 현재까지(2014-01-08) 실행 방법으로는 다음과 같다

(** 순서는 차례대로가 아닌 방법론의 구분임)    
- 1. 파워포인트와 이 프로그램을 관리자 권한으로 실행해야 함
- 2. 웹 주소 정보를 시스템에 등록한다
    (
        * 1회성 등록
        ** CMD를 관리자 권한으로 실행 후 다음의 명령을 실행.

        형식: netsh http add urlacl url=[URL 매핑 정보] user=[Application 실행 계정]
        등록 예: netsh http add urlacl url=http://+:80/SlideShow/ user="KimJaeYoung"           
        삭제 예 : netsh http delete urlacl url=http://+:80/SlideShow/     
    )
- 3. UAC를 끈다 (정말 비권장!!!)   

<<< 사용 >>>
솔루션 빌드 후 프로그램을 실행합니다.
원하는 PPT를 열고 프로그램에서 새로고침을 후 슬라이드 쑈 시작을 합니다.
원격 제어 할 기기에서(가령 핸드폰) 프로그램에서 안내한 주소를 입력하고
로그인 코드까지 입력하면 됩니다.
