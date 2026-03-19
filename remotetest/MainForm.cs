using System;
using System.Drawing;
using System.Windows.Forms;

namespace remotetest
{
    public partial class MainForm : Form
    {
        string sip;
        int sport;
        RemoteClientForm rcf = null;
        VirtualCursorForm vcf = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            vcf = new VirtualCursorForm();
            rcf = new RemoteClientForm();
            Remote.Singleton.RecvedRCInfo += new RecvRCInfoEventHandler(Remote_RecvedRCInfo);
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
                tbox_controller_ip.Text = e.IPAddressStr;
                sip = e.IPAddressStr;
                sport = e.Port;
                btn_ok.Enabled = true;
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
            Remote.Singleton.RecvEventStart();
            Remote.Singleton.StartImageRelay();
            timer_send_img.Start();
            vcf.Show();
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
