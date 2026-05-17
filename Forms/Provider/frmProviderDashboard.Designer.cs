using SERVIGO.Theme;

namespace SERVIGO.Forms.Provider
{
    partial class frmProviderDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize    = new Size(1280, 800);
            MinimumSize   = new Size(1100, 640);
            WindowState   = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = AppTheme.BgDark;
            Font          = AppTheme.FontBody;
            Text          = "SERVIGO – Provider Dashboard";
            ResumeLayout(false);
        }
    }
}
