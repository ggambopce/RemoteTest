using System;
using System.Drawing;
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

            // 원격제어 요청 수신 이벤트 메시지 핸들러 등록
            SetupServer.RecvedRCInfo +=
                new RecvRCInfoEventHandler(SetupServer_RecvedRCInfo);
            SetupServer.Start(MyIP, NetworkInfo.SetupPort);
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
        /// 메시지 수신 서버 가동
        /// </summary>
        public void RecvEventStart()
        {
            res = new RecvEventServer(MyIP, NetworkInfo.EventPort);
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
        }
    }
}
