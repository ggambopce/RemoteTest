using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace remotetest
{
    public class Controller
    {
        static Controller singleton;
        public static Controller Singleton
        {
            get
            {
                return singleton;
            }
        }
        static Controller()
        {
            singleton = new Controller();
        }
        private Controller()
        {

        }
        ImageServer img_sever = null;
        SendEventClient sce = null;
        public event RecvImageEventHandler RecvedImage = null;
        string host_ip;
        public SendEventClient SendEventClient
        {
            get
            {
                return sce;
            }
        }

        public string MyIP
        {
            get
            {
                string host_name = Dns.GetHostName();
                IPHostEntry host_entry = Dns.GetHostEntry(host_name);

                foreach (IPAddress ipaddr in host_entry.AddressList)
                {
                    if (ipaddr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ipaddr.ToString();
                    }
                }
                return string.Empty;
            }
        }


        public void Start(string host_ip, int port = 0)
        {
            this.host_ip = host_ip;
            int relayPort = port > 0 ? port : NetworkInfo.RelayPort;
            // 릴레이 서버에 CTRL_IMAGE로 먼저 연결 (호스트가 수락 후 이미지를 받을 준비)
            Socket imgSock = NetworkInfo.ConnectToRelay(host_ip, relayPort,
                                                        RelayRole.Ctrl, RelayChannel.Image);
            img_sever = new ImageServer(imgSock);
            img_sever.RecvedImage += new RecvImageEventHandler(img_sever_RecvedImage);
            // 릴레이를 통해 호스트에 연결 요청 전송
            SetupClient.SetupRelay(host_ip, relayPort);
        }
        void img_sever_RecvedImage(object sender, RecvImageEventArgs e)
        {
            if (RecvedImage != null)
            {
                RecvedImage(this, e);
            }
        }
        public void StartEventClient()
        {
            // 릴레이를 통해 이벤트 채널 연결
            Socket evtSock = NetworkInfo.ConnectToRelay(host_ip, NetworkInfo.RelayPort,
                                                        RelayRole.Ctrl, RelayChannel.Event);
            sce = new SendEventClient(evtSock);
        }
        public void Stop()
        {
            if (img_sever != null)
            {
                img_sever.Close();
                img_sever = null;
            }
            if (sce != null)
            {
                sce.Close();
                sce = null;
            }
        }
    }
}