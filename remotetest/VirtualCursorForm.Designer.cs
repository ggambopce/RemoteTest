namespace remotetest
{
    partial class VirtualCursorForm
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
            this.SuspendLayout();
            // VirtualCursorForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(10, 10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VirtualCursorForm";
            this.ShowInTaskbar = false;
            this.Text = "VirtualCursorForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VirtualCursorForm_Load);
            this.ResumeLayout(false);
        }
    }
}
