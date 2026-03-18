namespace remotetest
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tbox_ip = new System.Windows.Forms.TextBox();
            this.btn_setting = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbox_controller_ip = new System.Windows.Forms.TextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.timer_send_img = new System.Windows.Forms.Timer(this.components);
            this.noti = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.Text = "연결할 IP:";
            // tbox_ip
            this.tbox_ip.Location = new System.Drawing.Point(80, 17);
            this.tbox_ip.Name = "tbox_ip";
            this.tbox_ip.Size = new System.Drawing.Size(150, 21);
            this.tbox_ip.TabIndex = 0;
            // btn_setting
            this.btn_setting.Location = new System.Drawing.Point(240, 15);
            this.btn_setting.Name = "btn_setting";
            this.btn_setting.Size = new System.Drawing.Size(75, 23);
            this.btn_setting.TabIndex = 1;
            this.btn_setting.Text = "연결";
            this.btn_setting.UseVisualStyleBackColor = true;
            this.btn_setting.Click += new System.EventHandler(this.btn_setting_Click);
            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 12);
            this.label2.Text = "요청 IP:";
            // tbox_controller_ip
            this.tbox_controller_ip.Location = new System.Drawing.Point(80, 57);
            this.tbox_controller_ip.Name = "tbox_controller_ip";
            this.tbox_controller_ip.ReadOnly = true;
            this.tbox_controller_ip.Size = new System.Drawing.Size(150, 21);
            this.tbox_controller_ip.TabIndex = 2;
            // btn_ok
            this.btn_ok.Enabled = false;
            this.btn_ok.Location = new System.Drawing.Point(240, 55);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 3;
            this.btn_ok.Text = "수락";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // timer_send_img
            this.timer_send_img.Interval = 100;
            this.timer_send_img.Tick += new System.EventHandler(this.timer_send_img_Tick);
            // noti
            this.noti.Text = "remotetest";
            this.noti.Visible = true;
            this.noti.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.noti_MouseDoubleClick);
            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 217);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbox_ip);
            this.Controls.Add(this.btn_setting);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbox_controller_ip);
            this.Controls.Add(this.btn_ok);
            this.Name = "MainForm";
            this.Text = "Remote Control";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbox_ip;
        private System.Windows.Forms.Button btn_setting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbox_controller_ip;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Timer timer_send_img;
        private System.Windows.Forms.NotifyIcon noti;
    }
}
