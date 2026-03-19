using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;

namespace remotetest
{
    /// <summary>
    /// 원격 제어 호스트 클래스
    /// </summary>
    public class Remote
    {
        static Remote singleton;
        /// <summary>
        /// 단일 개체 - 가져오기
        /// </summary>
        public static Remote Singleton
        {
            get { return singleton; }
        }

        static Remote()
        {
            singleton = new Remote();
        }

        // 원격제어 요청 메시지 수신 이벤트
        public event RecvRCInfoEventHandler RecvedRCInfo = null;

        // 키보드, 마우스 메시지 수신 이벤트
        public event RecvKMEEventHandler RecvedKMEvent = null;

        RecvEventServer res = null;
        ImageClient imgClient = null;

        /// <summary>
        /// 데스크톱 사각 영역 - 가져오기
        /// </summary>
        public Rectangle Rect
        {
            get;
            private set;
        }

        private Remote()
        {
            // 주 화면의 전체 영역 구하기
            Rect = Screen.PrimaryScreen.Bounds;

            // 릴레이 서버 시작
            RelayServer.Start(NetworkInfo.RelayPort);

            // 원격제어 요청 수신 이벤트 메시지 핸들러 등록
            SetupServer.RecvedRCInfo +=
                new RecvRCInfoEventHandler(SetupServer_RecvedRCInfo);
            // 릴레이 서버를 통해 컨트롤러 연결 대기
            SetupServer.StartRelay("127.0.0.1", NetworkInfo.RelayPort);
        }

        void SetupServer_RecvedRCInfo(object sender, RecvRCInfoEventArgs e)
        {
            if (RecvedRCInfo != null)
                RecvedRCInfo(this, e);
        }

        /// <summary>
        /// 로컬 IP 문자열 - 가져오기
        /// </summary>
        public string MyIP
        {
            get { return NetworkInfo.DefaultIP; }
        }

        /// <summary>
        /// 릴레이를 통해 이미지 전송 시작 (컨트롤러 수락 후 호출)
        /// </summary>
        public void StartImageRelay()
        {
            Socket imgSock = NetworkInfo.ConnectToRelay("127.0.0.1", NetworkInfo.RelayPort,
                                                        RelayRole.Host, RelayChannel.Image);
            imgClient = new ImageClient(imgSock);
        }

        /// <summary>
        /// 이미지를 비동기로 전송
        /// </summary>
        public void SendImageAsync(System.Drawing.Image img)
        {
            imgClient?.SendImageAsync(img, null);
        }

        /// <summary>
        /// 메시지 수신 서버 가동
        /// </summary>
        public void RecvEventStart()
        {
            Socket evtSock = NetworkInfo.ConnectToRelay("127.0.0.1", NetworkInfo.RelayPort,
                                                        RelayRole.Host, RelayChannel.Event);
            res = new RecvEventServer(evtSock);
            res.RecvedKMEvent += new RecvKMEEventHandler(res_RecvKMEEventHandler);
        }

        void res_RecvKMEEventHandler(object sender, RecvKMEEventArgs e)
        {
            if (RecvedKMEvent != null)
                RecvedKMEvent(this, e);

            switch (e.MT)
            {
                case MsgType.MT_KDOWN: WrapNative.KeyDown(e.Key); break;
                case MsgType.MT_KEYUP: WrapNative.KeyUp(e.Key); break;
                case MsgType.MT_M_LEFTDOWN: WrapNative.LeftDown(); break;
                case MsgType.MT_M_LEFTUP: WrapNative.LeftUp(); break;
                case MsgType.MT_M_MOVE: WrapNative.Move(e.Now); break;
            }
        }

        /// <summary>
        /// 원격 제어 호스트 닫기
        /// </summary>
        public void Stop()
        {
            SetupServer.Close();
            if (res != null)
            {
                res.Close();
                res = null;
            }
            if (imgClient != null)
            {
                imgClient.Close();
                imgClient = null;
            }
            RelayServer.Close();
        }
    }
}
