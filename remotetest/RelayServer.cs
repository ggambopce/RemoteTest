using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace remotetest
{
    public enum RelayRole : byte { Host = 0, Ctrl = 1 }
    public enum RelayChannel : byte { Setup = 0, Image = 1, Event = 2 }

    /// <summary>
    /// TCP 릴레이 서버 - HOST와 CTRL 연결을 채널별로 중계
    /// </summary>
    public static class RelayServer
    {
        static Socket listenSock;
        static readonly object lck = new object();
        static Dictionary<RelayChannel, Socket> pendingHost = new Dictionary<RelayChannel, Socket>();
        static Dictionary<RelayChannel, Socket> pendingCtrl = new Dictionary<RelayChannel, Socket>();

        public static void Start(int port)
        {
            listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSock.Bind(new IPEndPoint(IPAddress.Any, port));
            listenSock.Listen(10);
            new Thread(AcceptLoop) { IsBackground = true, Name = "RelayAccept" }.Start();
        }

        static void AcceptLoop()
        {
            try
            {
                while (true)
                {
                    Socket client = listenSock.Accept();
                    ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
                }
            }
            catch { }
        }

        static void HandleClient(Socket sock)
        {
            try
            {
                // 2바이트 헤더 수신: [역할, 채널]
                byte[] header = new byte[2];
                int n = 0;
                while (n < 2) n += sock.Receive(header, n, 2 - n, SocketFlags.None);

                RelayRole role = (RelayRole)header[0];
                RelayChannel ch = (RelayChannel)header[1];
                bool isHost = (role == RelayRole.Host);

                Socket peer = null;
                lock (lck)
                {
                    var pendingPeer = isHost ? pendingCtrl : pendingHost;
                    var pendingSelf = isHost ? pendingHost : pendingCtrl;

                    if (pendingPeer.ContainsKey(ch))
                    {
                        peer = pendingPeer[ch];
                        pendingPeer.Remove(ch);
                    }
                    else
                    {
                        pendingSelf[ch] = sock;
                        return; // 상대방 연결 대기
                    }
                }

                Socket hostSock = isHost ? sock : peer;
                Socket ctrlSock = isHost ? peer : sock;

                if (ch == RelayChannel.Setup)
                {
                    // 컨트롤러 IP를 호스트에게 전달 후 종료
                    IPEndPoint ep = ctrlSock.RemoteEndPoint as IPEndPoint;
                    byte[] ipBytes = Encoding.ASCII.GetBytes(ep.Address.ToString() + "\n");
                    hostSock.Send(ipBytes);
                    hostSock.Close();
                    ctrlSock.Close();
                }
                else
                {
                    // 양방향 파이프 연결
                    Pipe(hostSock, ctrlSock);
                    Pipe(ctrlSock, hostSock);
                }
            }
            catch
            {
                try { sock.Close(); } catch { }
            }
        }

        static void Pipe(Socket from, Socket to)
        {
            new Thread(() =>
            {
                byte[] buf = new byte[65536];
                try
                {
                    while (true)
                    {
                        int n = from.Receive(buf);
                        if (n == 0) break;
                        int sent = 0;
                        while (sent < n) sent += to.Send(buf, sent, n - sent, SocketFlags.None);
                    }
                }
                catch { }
                finally
                {
                    try { from.Close(); } catch { }
                    try { to.Close(); } catch { }
                }
            })
            { IsBackground = true }.Start();
        }

        public static void Close()
        {
            try { listenSock?.Close(); } catch { }
        }
    }
}
