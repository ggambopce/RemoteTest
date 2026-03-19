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
    /// <summary>
    /// 원격 제어 이벤트 수신 서버
    /// </summary>
    public class RecvEventServer
    {
        Socket lis_sock;
        /// <summary>
        /// 원격 제어 이벤트 수신하였음을 알리는 이벤트
        /// </summary>
        public event RecvKMEEventHandler RecvedKMEvent = null;
        Thread th;
        /// <summary>
        /// 생성자 (직접 연결 모드)
        /// </summary>
        /// <param name="ip">로컬 IP</param>
        /// <param name="port">포트</param>
        public RecvEventServer(string ip, int port)
        {
            //소켓 생성
            lis_sock = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Parse(ip);
            IPEndPoint ep = new IPEndPoint(ipaddr, port);
            lis_sock.Bind(ep);//소켓과 IP 단말 개체 결합
            lis_sock.Listen(5); //Back 로그 큐 크기 설정
            //연결 요청 수락 쓰레드 시작
            ThreadStart ts = new ThreadStart(AcceptLoop);
            th = new Thread(ts);
            th.Start();
        }

        /// <summary>
        /// 생성자 (릴레이 모드) - 이미 연결된 릴레이 소켓으로 이벤트 수신
        /// </summary>
        /// <param name="relaySock">릴레이 서버와 연결된 소켓</param>
        public RecvEventServer(Socket relaySock)
        {
            relaySock_ = relaySock;
            th = new Thread(() => RelayReceiveLoop(relaySock)) { IsBackground = true };
            th.Start();
        }

        Socket relaySock_;

        void RelayReceiveLoop(Socket sock)
        {
            byte[] buffer = new byte[9];
            try
            {
                while (true)
                {
                    // 9바이트 이벤트 메시지를 완전히 수신
                    int received = 0;
                    while (received < 9)
                        received += sock.Receive(buffer, received, 9 - received, SocketFlags.None);

                    if (RecvedKMEvent != null)
                    {
                        RecvKMEEventArgs e = new RecvKMEEventArgs(new Meta(buffer));
                        RecvedKMEvent(this, e);
                    }
                }
            }
            catch { }
            finally
            {
                try { sock.Close(); } catch { }
            }
        }

        /// <summary>
        /// 비동기로 수신하기 위한 대리자
        /// </summary>
        /// <param name="dosock"></param>
        public delegate void ReceiveDele(Socket dosock);
        void AcceptLoop()
        {
            Socket do_sock;
            ReceiveDele rld = new ReceiveDele(Receive);//수신 대리자 개체 생성
            try
            {
                while (true)
                {
                    do_sock = lis_sock.Accept();
                    rld.BeginInvoke(do_sock, null, null);//비동기로 수신
                }
            }
            catch
            {
                Close();
            }
        }
        void Receive(Socket dosock)
        {
            byte[] buffer = new byte[9];//수신할 버퍼 생성
            int n = dosock.Receive(buffer);//메시지 수신
            if (RecvedKMEvent != null)//수신 이벤트 구독자가 있을 때
            {
                //이벤트 인자 생성
                RecvKMEEventArgs e = new RecvKMEEventArgs(new Meta(buffer));
                RecvedKMEvent(this, e);//수신 이벤트 통보(게시)
            }
            dosock.Close();//소켓 닫기
        }
        /// <summary>
        /// 원격 제어 이벤트 수신 서버 닫기
        /// </summary>
        public void Close()
        {
            if (lis_sock != null)
            {
                lis_sock.Close();//listen 소켓 닫기
                lis_sock = null;
            }
            if (relaySock_ != null)
            {
                try { relaySock_.Close(); } catch { }
                relaySock_ = null;
            }
        }
    }
}