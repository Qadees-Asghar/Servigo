using SERVIGO.Theme;

namespace SERVIGO.Forms
{
    partial class frmSignup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableMain       = new TableLayoutPanel();
            pnlLeft         = new Panel();
            pnlLeftAccent   = new Panel();
            flowLeft        = new FlowLayoutPanel();
            lblBrand        = new Label();
            lblBrandSub     = new Label();
            pnlRight        = new Panel();
            pnlScroll       = new Panel();
            flowForm        = new FlowLayoutPanel();
            lblFormTitle    = new Label();
            lblFormSub      = new Label();
            lblFullNameHdr  = new Label();
            txtFullName     = new TextBox();
            lblEmailHdr     = new Label();
            txtEmail        = new TextBox();
            lblPhoneHdr     = new Label();
            txtPhone        = new TextBox();
            lblCNICHdr      = new Label();
            txtCNIC         = new TextBox();
            lblPwdHdr       = new Label();
            txtPassword     = new TextBox();
            lblConfirmHdr   = new Label();
            txtConfirm      = new TextBox();
            lblRoleHdr      = new Label();
            pnlRole         = new FlowLayoutPanel();
            rbCustomer      = new RadioButton();
            rbProvider      = new RadioButton();
            pnlProviderExt  = new Panel();
            lblCatHdr       = new Label();
            cboCategory     = new ComboBox();
            lblDescHdr      = new Label();
            txtDescription  = new TextBox();
            lblError        = new Label();
            btnSignup       = new Button();
            lblUserID       = new Label();

            tableMain.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlScroll.SuspendLayout();
            pnlProviderExt.SuspendLayout();
            SuspendLayout();

            // ── tableMain ────────────────────────────────────────────────────
            tableMain.Dock        = DockStyle.Fill;
            tableMain.ColumnCount = 2;
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36f));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64f));
            tableMain.RowCount = 1;
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            tableMain.Controls.Add(pnlLeft,  0, 0);
            tableMain.Controls.Add(pnlRight, 1, 0);

            // ── pnlLeft ──────────────────────────────────────────────────────
            pnlLeft.Dock      = DockStyle.Fill;
            pnlLeft.BackColor = AppTheme.Surface;
            pnlLeft.Controls.Add(pnlLeftAccent);
            pnlLeft.Controls.Add(flowLeft);
            pnlLeft.Resize += (s, e) =>
            {
                flowLeft.Left = Math.Max(0, (pnlLeft.Width  - flowLeft.Width)  / 2);
                flowLeft.Top  = Math.Max(0, (pnlLeft.Height - flowLeft.Height) / 2);
            };

            // pnlLeftAccent (orange top bar)
            pnlLeftAccent.Dock      = DockStyle.Top;
            pnlLeftAccent.Height    = 4;
            pnlLeftAccent.BackColor = AppTheme.Accent;

            // ── flowLeft (brand + perks) ─────────────────────────────────────
            flowLeft.FlowDirection = FlowDirection.TopDown;
            flowLeft.AutoSize      = true;
            flowLeft.WrapContents  = false;
            flowLeft.BackColor     = Color.Transparent;
            flowLeft.Padding       = new Padding(28);
            flowLeft.Controls.Add(lblBrand);
            flowLeft.Controls.Add(lblBrandSub);

            // lblBrand
            lblBrand.Text      = "SERVIGO";
            lblBrand.Font      = new Font("Segoe UI", 34, FontStyle.Bold);
            lblBrand.ForeColor = AppTheme.Accent;
            lblBrand.AutoSize  = true;
            lblBrand.Margin    = new Padding(0, 0, 0, 6);

            // lblBrandSub
            lblBrandSub.Text      = "Join thousands of users\nand service professionals.";
            lblBrandSub.Font      = new Font("Segoe UI", 11);
            lblBrandSub.ForeColor = AppTheme.TextMuted;
            lblBrandSub.AutoSize  = true;
            lblBrandSub.Margin    = new Padding(0, 0, 0, 28);

            foreach (string perk in new[] { "✔  Free account signup",
                                             "✔  Book in seconds",
                                             "✔  Verified providers",
                                             "✔  Instant notifications" })
            {
                flowLeft.Controls.Add(new Label
                {
                    Text      = perk,
                    Font      = AppTheme.FontBody,
                    ForeColor = AppTheme.TextMuted,
                    AutoSize  = true,
                    BackColor = Color.Transparent,
                    Margin    = new Padding(0, 0, 0, 8)
                });
            }

            // ── pnlRight ─────────────────────────────────────────────────────
            pnlRight.Dock      = DockStyle.Fill;
            pnlRight.BackColor = AppTheme.BgDark;
            pnlRight.Controls.Add(pnlScroll);

            // ── pnlScroll (scrollable form area) ─────────────────────────────
            pnlScroll.Dock       = DockStyle.Fill;
            pnlScroll.AutoScroll = true;
            pnlScroll.BackColor  = Color.Transparent;
            pnlScroll.Padding    = new Padding(50, 28, 50, 28);
            pnlScroll.Controls.Add(flowForm);

            // ── flowForm (TopDown = no overlap between controls) ─────────────
            flowForm.FlowDirection = FlowDirection.TopDown;
            flowForm.AutoSize      = true;
            flowForm.WrapContents  = false;
            flowForm.BackColor     = Color.Transparent;
            // add all children
            flowForm.Controls.Add(lblFormTitle);
            flowForm.Controls.Add(lblFormSub);
            flowForm.Controls.Add(lblFullNameHdr);
            flowForm.Controls.Add(txtFullName);
            flowForm.Controls.Add(lblEmailHdr);
            flowForm.Controls.Add(txtEmail);
            flowForm.Controls.Add(lblPhoneHdr);
            flowForm.Controls.Add(txtPhone);
            flowForm.Controls.Add(lblCNICHdr);
            flowForm.Controls.Add(txtCNIC);
            flowForm.Controls.Add(lblPwdHdr);
            flowForm.Controls.Add(txtPassword);
            flowForm.Controls.Add(lblConfirmHdr);
            flowForm.Controls.Add(txtConfirm);
            flowForm.Controls.Add(lblRoleHdr);
            flowForm.Controls.Add(pnlRole);
            flowForm.Controls.Add(pnlProviderExt);
            flowForm.Controls.Add(lblError);
            flowForm.Controls.Add(btnSignup);
            flowForm.Controls.Add(lblUserID);

            // ── Form header labels ────────────────────────────────────────────
            lblFormTitle.Text      = "Create Account";
            lblFormTitle.Font      = new Font("Segoe UI", 22, FontStyle.Bold);
            lblFormTitle.ForeColor = AppTheme.Accent;
            lblFormTitle.AutoSize  = true;
            lblFormTitle.Margin    = new Padding(0, 0, 0, 4);

            lblFormSub.Text      = "Fill in the details below to get started.";
            lblFormSub.Font      = AppTheme.FontBody;
            lblFormSub.ForeColor = AppTheme.TextMuted;
            lblFormSub.AutoSize  = true;
            lblFormSub.Margin    = new Padding(0, 0, 0, 22);

            // ── Field header labels ───────────────────────────────────────────
            static Label MakeHdr(string text) => new()
            {
                Text      = text,
                Font      = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextLight,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 4)
            };

            static TextBox MakeInput(bool password = false) => new()
            {
                Size                  = new Size(400, 44),
                Font                  = AppTheme.FontInput,
                BackColor             = AppTheme.Surface2,
                ForeColor             = AppTheme.TextLight,
                BorderStyle           = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password,
                Margin                = new Padding(0, 0, 0, 14)
            };

            lblFullNameHdr = MakeHdr("Full Name");
            txtFullName    = MakeInput();
            lblEmailHdr    = MakeHdr("Email Address");
            txtEmail       = MakeInput();
            lblPhoneHdr    = MakeHdr("Phone Number  (11 digits, e.g. 03001234567)");
            txtPhone       = MakeInput();
            lblCNICHdr     = MakeHdr("CNIC  (13 digits, no dashes)");
            txtCNIC        = MakeInput();
            lblPwdHdr      = MakeHdr("Password  (letters + digits required)");
            txtPassword    = MakeInput(password: true);
            lblConfirmHdr  = MakeHdr("Confirm Password");
            txtConfirm     = MakeInput(password: true);

            // re-add in correct order after reassignment
            flowForm.Controls.Clear();
            flowForm.Controls.Add(lblFormTitle);
            flowForm.Controls.Add(lblFormSub);
            flowForm.Controls.Add(lblFullNameHdr);
            flowForm.Controls.Add(txtFullName);
            flowForm.Controls.Add(lblEmailHdr);
            flowForm.Controls.Add(txtEmail);
            flowForm.Controls.Add(lblPhoneHdr);
            flowForm.Controls.Add(txtPhone);
            flowForm.Controls.Add(lblCNICHdr);
            flowForm.Controls.Add(txtCNIC);
            flowForm.Controls.Add(lblPwdHdr);
            flowForm.Controls.Add(txtPassword);
            flowForm.Controls.Add(lblConfirmHdr);
            flowForm.Controls.Add(txtConfirm);
            flowForm.Controls.Add(lblRoleHdr);
            flowForm.Controls.Add(pnlRole);
            flowForm.Controls.Add(pnlProviderExt);
            flowForm.Controls.Add(lblError);
            flowForm.Controls.Add(btnSignup);
            flowForm.Controls.Add(lblUserID);

            // lblRoleHdr
            lblRoleHdr.Text      = "Account Type";
            lblRoleHdr.Font      = AppTheme.FontBodyBold;
            lblRoleHdr.ForeColor = AppTheme.TextLight;
            lblRoleHdr.AutoSize  = true;
            lblRoleHdr.Margin    = new Padding(0, 0, 0, 6);

            // ── pnlRole (radio buttons) ───────────────────────────────────────
            pnlRole.FlowDirection = FlowDirection.LeftToRight;
            pnlRole.AutoSize      = true;
            pnlRole.BackColor     = Color.Transparent;
            pnlRole.Margin        = new Padding(0, 0, 0, 10);
            pnlRole.Controls.Add(rbCustomer);
            pnlRole.Controls.Add(rbProvider);

            rbCustomer.Text      = "  Customer";
            rbCustomer.Font      = AppTheme.FontBody;
            rbCustomer.ForeColor = AppTheme.TextLight;
            rbCustomer.AutoSize  = true;
            rbCustomer.Checked   = true;
            rbCustomer.Margin    = new Padding(0, 0, 24, 0);

            rbProvider.Text      = "  Service Provider";
            rbProvider.Font      = AppTheme.FontBody;
            rbProvider.ForeColor = AppTheme.TextLight;
            rbProvider.AutoSize  = true;
            rbProvider.CheckedChanged += rbProvider_CheckedChanged;

            // ── pnlProviderExt (collapsed by default) ────────────────────────
            pnlProviderExt.AutoSize  = false;
            pnlProviderExt.Size      = new Size(400, 0);   // 0 height = collapsed
            pnlProviderExt.BackColor = Color.Transparent;
            pnlProviderExt.Visible   = false;
            pnlProviderExt.Margin    = new Padding(0);
            pnlProviderExt.Controls.Add(lblCatHdr);
            pnlProviderExt.Controls.Add(cboCategory);
            pnlProviderExt.Controls.Add(lblDescHdr);
            pnlProviderExt.Controls.Add(txtDescription);

            lblCatHdr.Text      = "Service Category";
            lblCatHdr.Font      = AppTheme.FontBodyBold;
            lblCatHdr.ForeColor = AppTheme.TextLight;
            lblCatHdr.AutoSize  = true;
            lblCatHdr.Location  = new Point(0, 0);

            cboCategory.Size          = new Size(400, 44);
            cboCategory.Font          = AppTheme.FontInput;
            cboCategory.BackColor     = AppTheme.Surface2;
            cboCategory.ForeColor     = AppTheme.TextLight;
            cboCategory.FlatStyle     = FlatStyle.Flat;
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Location      = new Point(0, 24);

            lblDescHdr.Text      = "About / Description";
            lblDescHdr.Font      = AppTheme.FontBodyBold;
            lblDescHdr.ForeColor = AppTheme.TextLight;
            lblDescHdr.AutoSize  = true;
            lblDescHdr.Location  = new Point(0, 78);

            txtDescription.Location    = new Point(0, 102);
            txtDescription.Size        = new Size(400, 68);
            txtDescription.Font        = AppTheme.FontInput;
            txtDescription.BackColor   = AppTheme.Surface2;
            txtDescription.ForeColor   = AppTheme.TextLight;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.Multiline   = true;

            // ── Error + submit ────────────────────────────────────────────────
            lblError.Text      = string.Empty;
            lblError.Font      = AppTheme.FontSmall;
            lblError.ForeColor = AppTheme.Danger;
            lblError.AutoSize  = true;
            lblError.Margin    = new Padding(0, 4, 0, 4);

            btnSignup.Text      = "CREATE ACCOUNT";
            btnSignup.Size      = new Size(400, 50);
            btnSignup.BackColor = AppTheme.Accent;
            btnSignup.ForeColor = Color.White;
            btnSignup.FlatStyle = FlatStyle.Flat;
            btnSignup.Font      = AppTheme.FontButton;
            btnSignup.Cursor    = Cursors.Hand;
            btnSignup.Margin    = new Padding(0, 8, 0, 14);
            btnSignup.FlatAppearance.BorderSize         = 0;
            btnSignup.FlatAppearance.MouseOverBackColor = AppTheme.AccentHover;
            btnSignup.Click += btnSignup_Click;

            lblUserID.Text      = string.Empty;
            lblUserID.Font      = AppTheme.FontBodyBold;
            lblUserID.ForeColor = AppTheme.Success;
            lblUserID.AutoSize  = true;

            // ── Form ─────────────────────────────────────────────────────────
            ClientSize      = new Size(1000, 700);
            MinimumSize     = new Size(900, 640);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = AppTheme.BgDark;
            Font            = AppTheme.FontBody;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            Text            = "SERVIGO – Create Account";
            Controls.Add(tableMain);

            pnlProviderExt.ResumeLayout(false);
            pnlScroll.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlLeft.ResumeLayout(false);
            tableMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Control declarations ──────────────────────────────────────────────
        private TableLayoutPanel tableMain;
        private Panel            pnlLeft;
        private Panel            pnlLeftAccent;
        private FlowLayoutPanel  flowLeft;
        private Label            lblBrand;
        private Label            lblBrandSub;
        private Panel            pnlRight;
        private Panel            pnlScroll;
        private FlowLayoutPanel  flowForm;
        private Label            lblFormTitle;
        private Label            lblFormSub;
        private Label            lblFullNameHdr;
        private TextBox          txtFullName;
        private Label            lblEmailHdr;
        private TextBox          txtEmail;
        private Label            lblPhoneHdr;
        private TextBox          txtPhone;
        private Label            lblCNICHdr;
        private TextBox          txtCNIC;
        private Label            lblPwdHdr;
        private TextBox          txtPassword;
        private Label            lblConfirmHdr;
        private TextBox          txtConfirm;
        private Label            lblRoleHdr;
        private FlowLayoutPanel  pnlRole;
        private RadioButton      rbCustomer;
        private RadioButton      rbProvider;
        private Panel            pnlProviderExt;
        private Label            lblCatHdr;
        private ComboBox         cboCategory;
        private Label            lblDescHdr;
        private TextBox          txtDescription;
        private Label            lblError;
        private Button           btnSignup;
        private Label            lblUserID;
    }
}
