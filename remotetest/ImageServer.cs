using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace remotetest
{
    /// <summary>
    /// 이미지 수신 서버
    /// </summary>
    public class ImageServer
    {
        Socket lis_sock; //Listening 소켓
        Thread accept_thread = null;

        /// <summary>
        /// 이미지 수신 이벤트
        /// </summary>
        public event RecvImageEventHandler RecvedImage = null;

        /// <summary>
        /// 생성자 (직접 연결 모드)
        /// </summary>
        /// <param name="ip">로컬 IP</param>
        /// <param name="port">포트</param>
        public ImageServer(string ip, int port)
        {
            //소켓 생성
            lis_sock = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);

            //EndPoint와 소켓 결합
            IPAddress ipaddr = IPAddress.Parse(ip);
            IPEndPoint ep = new IPEndPoint(ipaddr, port);
            lis_sock.Bind(ep);

            //Back 로그 큐 크기 설정
            lis_sock.Listen(5);

            //연결 수용 Loop을 수행하는 스레드 시작
            ThreadStart ts = new ThreadStart(AcceptLoop);
            accept_thread = new Thread(ts);
            accept_thread.Start();
        }

        /// <summary>
        /// 생성자 (릴레이 모드) - 이미 연결된 릴레이 소켓으로 이미지를 수신
        /// </summary>
        /// <param name="relaySock">릴레이 서버와 연결된 소켓</param>
        public ImageServer(Socket relaySock)
        {
            relaySock_ = relaySock;
            accept_thread = new Thread(() => RelayReceiveLoop(relaySock)) { IsBackground = true };
            accept_thread.Start();
        }

        Socket relaySock_;

        void RelayReceiveLoop(Socket sock)
        {
            try
            {
                while (true)
                {
                    // 4바이트 길이 헤더 수신
                    byte[] lbuf = new byte[4];
                    int n = 0;
                    while (n < 4) n += sock.Receive(lbuf, n, 4 - n, SocketFlags.None);
                    int len = BitConverter.ToInt32(lbuf, 0);

                    // 이미지 데이터 수신
                    byte[] buffer = new byte[len];
                    int trans = 0;
                    while (trans < len)
                        trans += sock.Receive(buffer, trans, len - trans, SocketFlags.None);

                    if (RecvedImage != null)
                    {
                        IPEndPoint iep = sock.RemoteEndPoint as IPEndPoint;
                        RecvImageEventArgs e = new RecvImageEventArgs(iep, ConvertBitmap(buffer));
                        RecvedImage(this, e);
                    }
                    // 소켓을 닫지 않고 다음 프레임 수신 대기
                }
            }
            catch { }
            finally
            {
                try { sock.Close(); } catch { }
            }
        }

        void AcceptLoop()
        {
            try
            {
                while (lis_sock != null)
                {
                    Socket do_sock = lis_sock.Accept();//연결 수락
                    Receive(do_sock);//do_sock으로 이미지 수신
                }
            }
            catch
            {
                Close();
            }
        }

        void Receive(Socket dosock)
        {
            byte[] lbuf = new byte[4]; //이미지 길이를 수신할 버퍼
            dosock.Receive(lbuf); //이미지 길이 수신
            int len = BitConverter.ToInt32(lbuf, 0);//수신한 버퍼의 내용을 정수로 변환
            byte[] buffer = new byte[len];//이미지 길이만큼의 버퍼 생성
            int trans = 0;
            while (trans < len)//수신할 이미지 데이터가 남아있으면
            {
                trans += dosock.Receive(buffer, trans,
                                        len - trans,
                                        SocketFlags.None);//이미지 수신
            }
            if (RecvedImage != null)//이미지 수신 이벤트가 존재하면
            {
                //이미지 수신 이벤트 발생
                IPEndPoint iep = dosock.RemoteEndPoint as IPEndPoint;
                RecvImageEventArgs e = new RecvImageEventArgs(iep, ConvertBitmap(buffer));
                RecvedImage(this, e);
            }
            dosock.Close();//소켓 닫기
        }
        /// <summary>
        /// 수신한 버퍼를 비트맵 이미지로 변환 메서드
        /// </summary>
        /// <param name="data">수신한 버퍼</param>
        /// <returns>비트맵 이미지</returns>
        public Bitmap ConvertBitmap(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, (int)data.Length);//메모리 스트림에 버퍼를 기록
            Bitmap bitmap = new Bitmap(ms); //메모리 스트림으로 비트맵 개체 생성
            return bitmap;
        }
        /// <summary>
        /// 비트맵 이미지 서버 닫기 메서드
        /// </summary>
        public void Close()
        {
            if (lis_sock != null)
            {
                lis_sock.Close();
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
