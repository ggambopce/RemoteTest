using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace remotetest
{
    public static class SetupServer
    {
        static Socket lis_sock; //연결 요청 수신 Listening 소켓
        static Thread accept_thread = null; //연결 요청 허용 스레드
        static volatile bool relayStopped = false;

        /// <summary>
        /// 연결 요청 수신 이벤트 핸들러
        /// </summary>
        static public event RecvRCInfoEventHandler RecvedRCInfo = null;

        /// <summary>
        /// 릴레이 서버를 통해 컨트롤러 연결을 대기하는 루프 시작
        /// </summary>
        static public void StartRelay(string relayIp, int relayPort)
        {
            relayStopped = false;
            new Thread(() => RelayLoop(relayIp, relayPort)) { IsBackground = true, Name = "SetupRelay" }.Start();
        }

        static void RelayLoop(string relayIp, int relayPort)
        {
            while (!relayStopped)
            {
                try
                {
                    Socket sock = NetworkInfo.ConnectToRelay(relayIp, relayPort, RelayRole.Host, RelayChannel.Setup);
                    // 릴레이 서버로부터 컨트롤러 IP 수신
                    byte[] buf = new byte[256];
                    int n = sock.Receive(buf);
                    sock.Close();

                    string ctrlIp = Encoding.ASCII.GetString(buf, 0, n).Trim();
                    if (RecvedRCInfo != null && ctrlIp.Length > 0)
                    {
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ctrlIp), 0);
                        RecvedRCInfo(null, new RecvRCInfoEventArgs(ep));
                    }
                }
                catch
                {
                    if (!relayStopped) Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 연결 요청 수신 서버 시작 메서드
        /// </summary>
        /// <param name="ip">서버의 IP주소</param>
        /// <param name="port">포트</param>
        static public void Start(string ip, int port)
        {
            //로컬 호스트의 IPEndPoint 개체 생성
            IPAddress ipaddr = IPAddress.Parse(ip);
            IPEndPoint ep = new IPEndPoint(ipaddr, port);
            //연결 요청 수신 Listening 소켓 생성
            lis_sock = new Socket(AddressFamily.InterNetwork, //네트워크 주소 체계
                SocketType.Stream,//전송 방식
                ProtocolType.Tcp);//프로토콜

            lis_sock.Bind(ep);//소켓과 IPEndPoint 결합
            lis_sock.Listen(1); //Back 로그 큐 크기 설정
            //연결 요청 허용 쓰레드 진입점 개체 생성
            ThreadStart ts = new ThreadStart(AcceptLoop);
            accept_thread = new Thread(ts); //연결 요청 허용 쓰레드 생성
            accept_thread.Start(); //연결 요청 허용 쓰레드 시작
        }

        static void AcceptLoop()
        {
            try
            {
                while (true)
                {
                    Socket do_sock = lis_sock.Accept();//연결 수락
                    if (RecvedRCInfo != null) //연결 요청 수신 이벤트 핸들러가 있을 때
                    {
                        RecvRCInfoEventArgs e = new RecvRCInfoEventArgs(
                                                do_sock.RemoteEndPoint);//이벤트 인자 생성
                        RecvedRCInfo(null, e); //이벤트 발생
                    }
                    do_sock.Close();//소켓 닫기
                }
            }
            catch
            {
                Close();
            }
        }
        /// <summary>
        /// 연결 요청 수신 서버 닫기
        /// </summary>
        public static void Close()
        {
            relayStopped = true;
            if (lis_sock != null)
            {
                lis_sock.Close();
                lis_sock = null;
            }
        }
    }
}
