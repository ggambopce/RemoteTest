namespace remotetest
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label_host = new System.Windows.Forms.Label();
            this.tbox_ip = new System.Windows.Forms.TextBox();
            this.btn_setting = new System.Windows.Forms.Button();
            this.label_controller = new System.Windows.Forms.Label();
            this.tbox_controller_ip = new System.Windows.Forms.TextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.timer_send_img = new System.Windows.Forms.Timer(this.components);
            this.noti = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            //
            // label_host
            //
            this.label_host.AutoSize = true;
            this.label_host.Location = new System.Drawing.Point(24, 40);
            this.label_host.Name = "label_host";
            this.label_host.Size = new System.Drawing.Size(88, 12);
            this.label_host.TabIndex = 0;
            this.label_host.Text = "원격 호스트 주소:";
            //
            // tbox_ip
            //
            this.tbox_ip.Location = new System.Drawing.Point(120, 37);
            this.tbox_ip.Name = "tbox_ip";
            this.tbox_ip.Size = new System.Drawing.Size(240, 21);
            this.tbox_ip.TabIndex = 1;
            //
            // btn_setting
            //
            this.btn_setting.Location = new System.Drawing.Point(374, 35);
            this.btn_setting.Name = "btn_setting";
            this.btn_setting.Size = new System.Drawing.Size(82, 25);
            this.btn_setting.TabIndex = 2;
            this.btn_setting.Text = "설정하기";
            this.btn_setting.UseVisualStyleBackColor = true;
            //
            // label_controller
            //
            this.label_controller.AutoSize = true;
            this.label_controller.Location = new System.Drawing.Point(12, 90);
            this.label_controller.Name = "label_controller";
            this.label_controller.Size = new System.Drawing.Size(100, 12);
            this.label_controller.TabIndex = 3;
            this.label_controller.Text = "원격 컨트롤러 주소:";
            //
            // tbox_controller_ip
            //
            this.tbox_controller_ip.Location = new System.Drawing.Point(120, 87);
            this.tbox_controller_ip.Name = "tbox_controller_ip";
            this.tbox_controller_ip.Size = new System.Drawing.Size(240, 21);
            this.tbox_controller_ip.TabIndex = 4;
            //
            // btn_ok
            //
            this.btn_ok.Location = new System.Drawing.Point(374, 85);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(82, 25);
            this.btn_ok.TabIndex = 5;
            this.btn_ok.Text = "원격 제어 허용";
            this.btn_ok.UseVisualStyleBackColor = true;
            //
            // timer_send_img
            //
            this.timer_send_img.Interval = 100;
            //
            // noti
            //
            this.noti.Text = "원격제어기";
            this.noti.Visible = false;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 160);
            this.Controls.Add(this.label_host);
            this.Controls.Add(this.tbox_ip);
            this.Controls.Add(this.btn_setting);
            this.Controls.Add(this.label_controller);
            this.Controls.Add(this.tbox_controller_ip);
            this.Controls.Add(this.btn_ok);
            this.Name = "MainForm";
            this.Text = "원격제어기";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_host;
        private System.Windows.Forms.TextBox tbox_ip;
        private System.Windows.Forms.Button btn_setting;
        private System.Windows.Forms.Label label_controller;
        private System.Windows.Forms.TextBox tbox_controller_ip;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Timer timer_send_img;
        private System.Windows.Forms.NotifyIcon noti;
    }
}
