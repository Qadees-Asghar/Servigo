using SERVIGO.Theme;

namespace SERVIGO.Forms
{
    partial class frmIntro
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
            pnlTopBar    = new Panel();
            flowLeft     = new FlowLayoutPanel();
            lblLogo      = new Label();
            lblLogoSub   = new Label();
            lblDesc      = new Label();
            flowChips    = new FlowLayoutPanel();
            pnlRight     = new Panel();
            flowRight    = new FlowLayoutPanel();
            lblWelcome   = new Label();
            lblWelcomeSub = new Label();
            btnLogin     = new Button();
            btnSignup    = new Button();

            tableMain.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            SuspendLayout();

            // ── tableMain ────────────────────────────────────────────────────
            tableMain.Dock        = DockStyle.Fill;
            tableMain.ColumnCount = 2;
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
            tableMain.RowCount = 1;
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            tableMain.Controls.Add(pnlLeft,  0, 0);
            tableMain.Controls.Add(pnlRight, 1, 0);

            // ── pnlLeft ──────────────────────────────────────────────────────
            pnlLeft.Dock      = DockStyle.Fill;
            pnlLeft.BackColor = AppTheme.Surface;
            pnlLeft.Controls.Add(pnlTopBar);
            pnlLeft.Controls.Add(flowLeft);
            pnlLeft.Resize += (s, e) =>
            {
                flowLeft.Left = Math.Max(0, (pnlLeft.Width  - flowLeft.Width)  / 2);
                flowLeft.Top  = Math.Max(0, (pnlLeft.Height - flowLeft.Height) / 2);
            };

            // pnlTopBar (orange accent bar)
            pnlTopBar.Dock      = DockStyle.Top;
            pnlTopBar.Height    = 3;
            pnlTopBar.BackColor = AppTheme.Accent;

            // ── flowLeft ─────────────────────────────────────────────────────
            flowLeft.FlowDirection = FlowDirection.TopDown;
            flowLeft.AutoSize      = true;
            flowLeft.WrapContents  = false;
            flowLeft.BackColor     = Color.Transparent;
            flowLeft.Padding       = new Padding(52, 0, 40, 0);
            flowLeft.Controls.Add(lblLogo);
            flowLeft.Controls.Add(lblLogoSub);
            flowLeft.Controls.Add(lblDesc);
            flowLeft.Controls.Add(flowChips);

            // lblLogo
            lblLogo.Text      = "SERVIGO";
            lblLogo.Font      = new Font("Segoe UI", 48, FontStyle.Bold);
            lblLogo.ForeColor = AppTheme.Accent;
            lblLogo.AutoSize  = true;
            lblLogo.Margin    = new Padding(0, 0, 0, 4);

            // lblLogoSub
            lblLogoSub.Text      = "Smart Appointment Booking System";
            lblLogoSub.Font      = new Font("Segoe UI", 13);
            lblLogoSub.ForeColor = AppTheme.TextMuted;
            lblLogoSub.AutoSize  = true;
            lblLogoSub.Margin    = new Padding(0, 0, 0, 30);

            // lblDesc
            lblDesc.Text      = "Connect with trusted service professionals.\n" +
                                 "Book electricians, plumbers, mechanics,\n" +
                                 "laundry workers & many more ";
            lblDesc.Font      = AppTheme.FontBody;
            lblDesc.ForeColor = AppTheme.TextMuted;
            lblDesc.AutoSize  = true;
            lblDesc.Margin    = new Padding(0, 0, 0, 32);

            // ── flowChips (filled in code via AddServiceChips) ────────────────
            flowChips.FlowDirection = FlowDirection.LeftToRight;
            flowChips.AutoSize      = true;
            flowChips.WrapContents  = true;
            flowChips.BackColor     = Color.Transparent;
            flowChips.MaximumSize   = new Size(520, 0);

            // ── pnlRight ─────────────────────────────────────────────────────
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
            flowRight.Padding       = new Padding(50, 0, 50, 0);
            flowRight.Controls.Add(lblWelcome);
            flowRight.Controls.Add(lblWelcomeSub);
            flowRight.Controls.Add(btnLogin);
            flowRight.Controls.Add(btnSignup);

            // lblWelcome
            lblWelcome.Text      = "Welcome";
            lblWelcome.Font      = new Font("Segoe UI", 30, FontStyle.Bold);
            lblWelcome.ForeColor = AppTheme.TextLight;
            lblWelcome.AutoSize  = true;
            lblWelcome.Margin    = new Padding(0, 0, 0, 6);

            // lblWelcomeSub
            lblWelcomeSub.Text      = "Login or create a new account\nto access the booking system.";
            lblWelcomeSub.Font      = AppTheme.FontBody;
            lblWelcomeSub.ForeColor = AppTheme.TextMuted;
            lblWelcomeSub.AutoSize  = true;
            lblWelcomeSub.Margin    = new Padding(0, 0, 0, 34);

            // btnLogin
            btnLogin.Text      = "  LOGIN TO MY ACCOUNT";
            btnLogin.Size      = new Size(320, 50);
            btnLogin.BackColor = AppTheme.Accent;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font      = AppTheme.FontButton;
            btnLogin.Cursor    = Cursors.Hand;
            btnLogin.Margin    = new Padding(0, 0, 0, 14);
            btnLogin.FlatAppearance.BorderSize         = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = AppTheme.AccentHover;
            btnLogin.Click += btnLogin_Click;

            // btnSignup
            btnSignup.Text      = "  CREATE NEW ACCOUNT";
            btnSignup.Size      = new Size(320, 46);
            btnSignup.BackColor = Color.Transparent;
            btnSignup.ForeColor = AppTheme.TextMuted;
            btnSignup.FlatStyle = FlatStyle.Flat;
            btnSignup.Font      = AppTheme.FontButton;
            btnSignup.Cursor    = Cursors.Hand;
            btnSignup.Margin    = new Padding(0, 0, 0, 0);
            btnSignup.FlatAppearance.BorderSize  = 1;
            btnSignup.FlatAppearance.BorderColor = AppTheme.Border;
            btnSignup.FlatAppearance.MouseOverBackColor = AppTheme.Surface2;
            btnSignup.Click += btnSignup_Click;

            // ── Form ─────────────────────────────────────────────────────────
            Text          = "SERVIGO -Smart Appointment Booking";
            WindowState   = FormWindowState.Maximized;
            MinimumSize   = new Size(1024, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = AppTheme.BgDark;
            Font          = AppTheme.FontBody;
            Controls.Add(tableMain);

            pnlLeft.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            tableMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Control declarations ──────────────────────────────────────────────
        private TableLayoutPanel tableMain;
        private Panel            pnlLeft;
        private Panel            pnlTopBar;
        private FlowLayoutPanel  flowLeft;
        private Label            lblLogo;
        private Label            lblLogoSub;
        private Label            lblDesc;
        private FlowLayoutPanel  flowChips;
        private Panel            pnlRight;
        private FlowLayoutPanel  flowRight;
        private Label            lblWelcome;
        private Label            lblWelcomeSub;
        private Button           btnLogin;
        private Button           btnSignup;
    }
}
