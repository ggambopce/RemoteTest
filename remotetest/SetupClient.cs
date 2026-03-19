using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace remotetest
{
    ///
    /// 원격 제어 요청 클라이언트 - 정적 클래스
    /// 
    public static class SetupClient
    {
        public static event EventHandler ConnectedEventHandler = null;
        public static event EventHandler ConnectFailedEventHandler = null;
        static Socket sock;
        ///
        /// 릴레이 서버를 통해 원격 제어 요청을 전달하는 메서드
        ///
        public static void SetupRelay(string relayIp, int relayPort)
        {
            try
            {
                Socket s = NetworkInfo.ConnectToRelay(relayIp, relayPort, RelayRole.Ctrl, RelayChannel.Setup);
                // 릴레이가 호스트에 IP를 전달 후 소켓을 닫음
                s.Close();
                if (ConnectedEventHandler != null)
                    ConnectedEventHandler(null, EventArgs.Empty);
            }
            catch
            {
                if (ConnectFailedEventHandler != null)
                    ConnectFailedEventHandler(null, EventArgs.Empty);
            }
        }

        ///
        /// 원격 제어 요청 메서드 (직접 연결 모드)
        ///
        ///상대 IP 주소
        ///상대 포트 번호
        public static void Setup(string ip, int port)
        {
            IPAddress ipaddr = IPAddress.Parse(ip); //상대 IP 주소 개체를 생성
            IPEndPoint ep = new IPEndPoint(ipaddr, port); //IP 단말 주소(IP주소 + 포트번호) 개체 생성
            //TCP 소켓 생성(네트워크 주소 체계, 전송 방식, 프로토콜)
            sock = new Socket(AddressFamily.InterNetwork, //네트워크 주소 체계
                SocketType.Stream, //전송 방식
                ProtocolType.Tcp);//프로토콜
            //sock.Connect(ip, port);
            sock.BeginConnect(ep, DoConnect, sock);
        }
        static void DoConnect(IAsyncResult result)
        {
            try
            {
                sock.EndConnect(result);
                if (ConnectedEventHandler != null)
                {
                    ConnectedEventHandler(null, new EventArgs());//연결 성공
                }
            }
            catch
            {
                if (ConnectFailedEventHandler != null)
                {
                    ConnectFailedEventHandler(null, new EventArgs());//연결 실패
                }
            }
            sock.Close();
        }
    }
}
