using SERVIGO.Theme;

namespace SERVIGO.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableMain    = new TableLayoutPanel();
            pnlLeft      = new Panel();
            pnlAccentBar = new Panel();
            flowLeft     = new FlowLayoutPanel();
            lblBrand     = new Label();
            lblTagline   = new Label();
            pnlRight     = new Panel();
            flowRight    = new FlowLayoutPanel();
            lblWelcome   = new Label();
            lblSubtitle  = new Label();
            lblEmailHdr  = new Label();
            txtEmail     = new TextBox();
            lblUserIDHdr = new Label();
            txtUserID    = new TextBox();
            lblPwdHdr    = new Label();
            pnlPwd       = new Panel();
            txtPassword  = new TextBox();
            btnToggle    = new Button();
            lblError     = new Label();
            btnLogin     = new Button();
            pnlLink      = new FlowLayoutPanel();
            lblNoAccount = new Label();
            lnkSignup    = new LinkLabel();

            tableMain.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlPwd.SuspendLayout();
            pnlLink.SuspendLayout();
            SuspendLayout();

            // ── tableMain ────────────────────────────────────────────────────
            tableMain.Dock        = DockStyle.Fill;
            tableMain.ColumnCount = 2;
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));
            tableMain.RowCount = 1;
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            tableMain.Controls.Add(pnlLeft,  0, 0);
            tableMain.Controls.Add(pnlRight, 1, 0);

            // ── pnlLeft (dark Surface) ───────────────────────────────────────
            pnlLeft.Dock      = DockStyle.Fill;
            pnlLeft.BackColor = AppTheme.Surface;
            pnlLeft.Controls.Add(pnlAccentBar);
            pnlLeft.Controls.Add(flowLeft);
            pnlLeft.Resize += (s, e) =>
            {
                flowLeft.Left = Math.Max(0, (pnlLeft.Width  - flowLeft.Width)  / 2);
                flowLeft.Top  = Math.Max(0, (pnlLeft.Height - flowLeft.Height) / 2);
            };

            // ── pnlAccentBar (orange right border) ───────────────────────────
            pnlAccentBar.Dock      = DockStyle.Right;
            pnlAccentBar.Width     = 4;
            pnlAccentBar.BackColor = AppTheme.Accent;

            // ── flowLeft ─────────────────────────────────────────────────────
            flowLeft.FlowDirection = FlowDirection.TopDown;
            flowLeft.AutoSize      = true;
            flowLeft.WrapContents  = false;
            flowLeft.BackColor     = Color.Transparent;
            flowLeft.Padding       = new Padding(30);
            flowLeft.Controls.Add(lblBrand);
            flowLeft.Controls.Add(lblTagline);

            // lblBrand
            lblBrand.Text      = "SERVIGO";
            lblBrand.Font      = new Font("Segoe UI", 36, FontStyle.Bold);
            lblBrand.ForeColor = AppTheme.Accent;
            lblBrand.AutoSize  = true;
            lblBrand.Margin    = new Padding(0, 0, 0, 10);

            // lblTagline
            lblTagline.Text      = "Smart Appointment\nBooking System";
            lblTagline.Font      = new Font("Segoe UI", 12);
            lblTagline.ForeColor = AppTheme.TextMuted;
            lblTagline.AutoSize  = true;

            // ── pnlRight (BgDark) ────────────────────────────────────────────
            pnlRight.Dock      = DockStyle.Fill;
            pnlRight.BackColor = AppTheme.BgDark;
            pnlRight.Controls.Add(flowRight);
            pnlRight.Resize += (s, e) =>
            {
                flowRight.Left = Math.Max(0, (pnlRight.Width  - flowRight.Width)  / 2);
                flowRight.Top  = Math.Max(0, (pnlRight.Height - flowRight.Height) / 2);
            };

            // ── flowRight ────────────────────────────────────────────────────
            flowRight.FlowDirection = FlowDirection.TopDown;
            flowRight.AutoSize      = true;
            flowRight.WrapContents  = false;
            flowRight.BackColor     = Color.Transparent;
            flowRight.Controls.Add(lblWelcome);
            flowRight.Controls.Add(lblSubtitle);
            flowRight.Controls.Add(lblEmailHdr);
            flowRight.Controls.Add(txtEmail);
            flowRight.Controls.Add(lblUserIDHdr);
            flowRight.Controls.Add(txtUserID);
            flowRight.Controls.Add(lblPwdHdr);
            flowRight.Controls.Add(pnlPwd);
            flowRight.Controls.Add(lblError);
            flowRight.Controls.Add(btnLogin);
            flowRight.Controls.Add(pnlLink);

            // lblWelcome
            lblWelcome.Text      = "Welcome Back";
            lblWelcome.Font      = new Font("Segoe UI", 24, FontStyle.Bold);
            lblWelcome.ForeColor = AppTheme.Accent;
            lblWelcome.AutoSize  = true;
            lblWelcome.Margin    = new Padding(0, 0, 0, 4);

            // lblSubtitle
            lblSubtitle.Text      = "Enter your credentials to continue";
            lblSubtitle.Font      = AppTheme.FontBody;
            lblSubtitle.ForeColor = AppTheme.TextMuted;
            lblSubtitle.AutoSize  = true;
            lblSubtitle.Margin    = new Padding(0, 0, 0, 26);

            // lblEmailHdr
            lblEmailHdr.Text      = "Email Address";
            lblEmailHdr.Font      = AppTheme.FontBodyBold;
            lblEmailHdr.ForeColor = AppTheme.TextLight;
            lblEmailHdr.AutoSize  = true;
            lblEmailHdr.Margin    = new Padding(0, 0, 0, 4);

            // txtEmail
            txtEmail.Size        = new Size(360, 44);
            txtEmail.Font        = AppTheme.FontInput;
            txtEmail.BackColor   = AppTheme.Surface2;
            txtEmail.ForeColor   = AppTheme.TextLight;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Margin      = new Padding(0, 0, 0, 14);

            // lblUserIDHdr
            lblUserIDHdr.Text      = "User ID";
            lblUserIDHdr.Font      = AppTheme.FontBodyBold;
            lblUserIDHdr.ForeColor = AppTheme.TextLight;
            lblUserIDHdr.AutoSize  = true;
            lblUserIDHdr.Margin    = new Padding(0, 0, 0, 4);

            // txtUserID
            txtUserID.Size            = new Size(360, 44);
            txtUserID.Font            = AppTheme.FontInput;
            txtUserID.BackColor       = AppTheme.Surface2;
            txtUserID.ForeColor       = AppTheme.TextLight;
            txtUserID.BorderStyle     = BorderStyle.FixedSingle;
            txtUserID.CharacterCasing = CharacterCasing.Upper;
            txtUserID.Margin          = new Padding(0, 0, 0, 14);

            // lblPwdHdr
            lblPwdHdr.Text      = "Password";
            lblPwdHdr.Font      = AppTheme.FontBodyBold;
            lblPwdHdr.ForeColor = AppTheme.TextLight;
            lblPwdHdr.AutoSize  = true;
            lblPwdHdr.Margin    = new Padding(0, 0, 0, 4);

            // ── pnlPwd (password + toggle in a fixed-size row) ───────────────
            pnlPwd.Size      = new Size(360, 44);
            pnlPwd.BackColor = Color.Transparent;
            pnlPwd.Margin    = new Padding(0, 0, 0, 8);
            pnlPwd.Controls.Add(txtPassword);
            pnlPwd.Controls.Add(btnToggle);

            // txtPassword
            txtPassword.Location              = new Point(0, 0);
            txtPassword.Size                  = new Size(316, 44);
            txtPassword.Font                  = AppTheme.FontInput;
            txtPassword.BackColor             = AppTheme.Surface2;
            txtPassword.ForeColor             = AppTheme.TextLight;
            txtPassword.BorderStyle           = BorderStyle.FixedSingle;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Anchor                = AnchorStyles.Left | AnchorStyles.Top;

            // btnToggle
            btnToggle.Location  = new Point(318, 0);
            btnToggle.Size      = new Size(42, 44);
            btnToggle.Text      = "👁";
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.BackColor = AppTheme.Surface2;
            btnToggle.ForeColor = AppTheme.TextMuted;
            btnToggle.Cursor    = Cursors.Hand;
            btnToggle.TabStop   = false;
            btnToggle.Anchor    = AnchorStyles.Right | AnchorStyles.Top;
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += btnToggle_Click;

            // lblError
            lblError.Text      = string.Empty;
            lblError.Font      = AppTheme.FontSmall;
            lblError.ForeColor = AppTheme.Danger;
            lblError.AutoSize  = true;
            lblError.Margin    = new Padding(0, 4, 0, 4);

            // btnLogin
            btnLogin.Text      = "LOGIN";
            btnLogin.Size      = new Size(360, 48);
            btnLogin.BackColor = AppTheme.Accent;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font      = AppTheme.FontButton;
            btnLogin.Cursor    = Cursors.Hand;
            btnLogin.Margin    = new Padding(0, 8, 0, 18);
            btnLogin.FlatAppearance.BorderSize         = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = AppTheme.AccentHover;
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 90, 10);
            btnLogin.Click += btnLogin_Click;

            // pnlLink
            pnlLink.FlowDirection = FlowDirection.LeftToRight;
            pnlLink.AutoSize      = true;
            pnlLink.BackColor     = Color.Transparent;
            pnlLink.Controls.Add(lblNoAccount);
            pnlLink.Controls.Add(lnkSignup);

            // lblNoAccount
            lblNoAccount.Text      = "Don't have an account?  ";
            lblNoAccount.Font      = AppTheme.FontBody;
            lblNoAccount.ForeColor = AppTheme.TextMuted;
            lblNoAccount.AutoSize  = true;

            // lnkSignup
            lnkSignup.Text      = "Sign up here";
            lnkSignup.Font      = AppTheme.FontBodyBold;
            lnkSignup.AutoSize  = true;
            lnkSignup.LinkColor = AppTheme.Accent;
            lnkSignup.LinkClicked += lnkSignup_LinkClicked;

            // ── Form ─────────────────────────────────────────────────────────
            ClientSize      = new Size(940, 620);
            MinimumSize     = new Size(800, 560);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = AppTheme.BgDark;
            Font            = AppTheme.FontBody;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            Text            = "SERVIGO – Login";
            Controls.Add(tableMain);

            pnlPwd.ResumeLayout(false);
            pnlLink.ResumeLayout(false);
            pnlLeft.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            tableMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Control declarations ──────────────────────────────────────────────
        private TableLayoutPanel tableMain;
        private Panel            pnlLeft;
        private Panel            pnlAccentBar;
        private FlowLayoutPanel  flowLeft;
        private Label            lblBrand;
        private Label            lblTagline;
        private Panel            pnlRight;
        private FlowLayoutPanel  flowRight;
        private Label            lblWelcome;
        private Label            lblSubtitle;
        private Label            lblEmailHdr;
        private TextBox          txtEmail;
        private Label            lblUserIDHdr;
        private TextBox          txtUserID;
        private Label            lblPwdHdr;
        private Panel            pnlPwd;
        private TextBox          txtPassword;
        private Button           btnToggle;
        private Label            lblError;
        private Button           btnLogin;
        private FlowLayoutPanel  pnlLink;
        private Label            lblNoAccount;
        private LinkLabel        lnkSignup;
    }
}
