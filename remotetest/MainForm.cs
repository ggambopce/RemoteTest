using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace remotetest
{
    public partial class MainForm : Form
    {
        string sip;
        int sport;
        RemoteClientForm rcf = null;
        VirtualCursorForm vcf = null;
        private readonly bool _isHostMode;
        private readonly string _autoRelayIp;
        private readonly int _autoRelayPort;

        public MainForm(bool isHostMode = false, string relayIp = null, int relayPort = 20020)
        {
            InitializeComponent();
            _isHostMode = isHostMode;
            _autoRelayIp = relayIp;
            _autoRelayPort = relayPort;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            vcf = new VirtualCursorForm();
            rcf = new RemoteClientForm();

            if (_isHostMode)
            {
                // 호스트 모드 (VM): 수락 대기 즉시 시작 (테스트 - 세션 폴링 없음)
                this.Text = "원격 제어 - 호스트 모드";

                // 모든 UI 숨기고 최소화
                label1.Visible = false;
                tbox_ip.Visible = false;
                btn_setting.Visible = false;
                label2.Visible = false;
                tbox_controller_ip.Visible = false;
                btn_ok.Visible = false;
                label3.Visible = false;
                tbox_session_key.Visible = false;
                btn_connect_api.Visible = false;
                lbl_api_status.Visible = false;
                this.ClientSize = new System.Drawing.Size(300, 40);

                // 컨트롤러가 Setup 채널로 연결되면 이미지 전송 시작
                Remote.Singleton.RecvedRCInfo += new RecvRCInfoEventHandler(Remote_RecvedRCInfo);

                // API 서버 등록 + 하트비트 시작
                Remote.Singleton.StartApiMode(AgentConfig.ApiBaseUrl);

                // 컨트롤러 연결 대기 (이미지 채널은 컨트롤러 연결 후 시작)
                this.Shown += (s, ev) =>
                {
                    Remote.Singleton.BeginAcceptingControllers();
                    Remote.Singleton.RecvEventStart();
                };
            }
            else
            {
                // 컨트롤러 모드 (PC): 세션 키 입력으로 연결
                this.Text = "원격 제어 - 컨트롤러 모드";

                // 호스트 섹션 숨기기
                label1.Visible = false;
                tbox_ip.Visible = false;
                btn_setting.Visible = false;
                label2.Visible = false;
                tbox_controller_ip.Visible = false;
                btn_ok.Visible = false;

                // 세션 키 섹션을 위쪽으로 재배치
                label3.Location = new System.Drawing.Point(12, 20);
                tbox_session_key.Location = new System.Drawing.Point(80, 17);
                btn_connect_api.Location = new System.Drawing.Point(240, 15);
                lbl_api_status.Location = new System.Drawing.Point(80, 50);
                this.ClientSize = new System.Drawing.Size(340, 75);

                // vdesk://IP:PORT 로 실행된 경우 자동 연결
                if (!string.IsNullOrEmpty(_autoRelayIp))
                {
                    this.Shown += (s, ev) => ConnectToRelay(_autoRelayIp, _autoRelayPort);
                }
            }
        }

        delegate void SessionAcceptedDele(object sender, string sessionKey);
        void Remote_SessionAccepted(object sender, string sessionKey)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SessionAcceptedDele(Remote_SessionAccepted), sender, sessionKey);
                return;
            }
            // API 모드로 자동 수락됨 — 이미지 전송 타이머 시작
            timer_send_img.Start();
        }

        delegate void Remote_Dele(object sender, RecvRCInfoEventArgs e);
        void Remote_RecvedRCInfo(object sender, RecvRCInfoEventArgs e)
        {
            if (this.InvokeRequired)
            {
                object[] objs = new object[2] { sender, e };
                this.Invoke(new Remote_Dele(Remote_RecvedRCInfo), objs);
            }
            else
            {
                if (_isHostMode)
                {
                    // 컨트롤러가 연결됨 → 이미지 릴레이 시작
                    Remote.Singleton.StartImageRelay();
                    timer_send_img.Start();
                }
                else
                {
                    tbox_controller_ip.Text = e.IPAddressStr;
                    sip = e.IPAddressStr;
                    sport = e.Port;
                    btn_ok.Enabled = true;
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Remote.Singleton.Stop();
            Controller.Singleton.Stop();
            Application.Exit();
        }

        private void btn_setting_Click(object sender, EventArgs e)
        {
            if (tbox_ip.Text == NetworkInfo.DefaultIP)
            {
                MessageBox.Show("같은 호스트를 원격 제어할 수 없습니다.");
                tbox_ip.Text = string.Empty;
                return;
            }

            string host_ip = tbox_ip.Text;
            Rectangle rect = Remote.Singleton.Rect;
            Controller.Singleton.Start(host_ip);

            rcf.ClientSize = new Size(rect.Width - 40, rect.Height - 80);
            rcf.Show();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Hide();
            Remote.Singleton.BeginAcceptingControllers();
            Remote.Singleton.RecvEventStart();
            Remote.Singleton.StartImageRelay();
            timer_send_img.Start();
            vcf.Show();
        }

        private void btn_connect_api_Click(object sender, EventArgs e)
        {
            string input = tbox_session_key.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            // IP:PORT 또는 IP 형태 모두 지원
            string ip = input;
            int port = 20020;
            int colonIdx = input.LastIndexOf(':');
            if (colonIdx > 0 && int.TryParse(input.Substring(colonIdx + 1), out int parsedPort))
            {
                ip = input.Substring(0, colonIdx);
                port = parsedPort;
            }

            ConnectToRelay(ip, port);
        }

        private void ConnectToRelay(string ip, int port)
        {
            lbl_api_status.Text = "연결 중...";
            try
            {
                Controller.Singleton.Start(ip, port);
                lbl_api_status.Text = "연결됨: " + ip + ":" + port;
                Rectangle rect = Remote.Singleton.Rect;
                rcf.ClientSize = new Size(rect.Width - 40, rect.Height - 80);
                rcf.Show();
            }
            catch (Exception ex)
            {
                lbl_api_status.Text = "연결 실패: " + ex.Message;
            }
        }

        private void noti_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void timer_send_img_Tick(object sender, EventArgs e)
        {
            Rectangle rect = Remote.Singleton.Rect;
            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
            Graphics gp = Graphics.FromImage(bitmap);
            Size size2 = new Size(rect.Width, rect.Height);
            gp.CopyFromScreen(new Point(0, 0), new Point(0, 0), size2);
            gp.Dispose();
            try
            {
                Remote.Singleton.SendImageAsync(bitmap);
            }
            catch
            {
                timer_send_img.Stop();
                MessageBox.Show("서버에 연결 실패");
                this.Close();
            }
        }
    }
}
