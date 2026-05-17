using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Theme;

namespace SERVIGO.Forms
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            AppTheme.AddPlaceholder(txtEmail,  "your@email.com");
            AppTheme.AddPlaceholder(txtUserID, "SRV-XXXXX");
        }

        private void btnToggle_Click(object? sender, EventArgs e)
            => txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

        private void lnkSignup_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            using var frm = new frmSignup();
            if (frm.ShowDialog(this) == DialogResult.OK)
                MessageBox.Show("Account created! Please login now.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            lblError.Text = string.Empty;

            string email    = AppTheme.GetText(txtEmail,  "your@email.com");
            string userID   = AppTheme.GetText(txtUserID, "SRV-XXXXX").ToUpper();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(userID) ||
                string.IsNullOrWhiteSpace(password))
            { ShowError("All fields are required."); return; }

            btnLogin.Enabled = false;
            btnLogin.Text    = "Logging in…";

            try
            {
                var user = UserDAL.Login(email, userID, password);
                if (user == null) { ShowError("Invalid email, user ID, or password."); return; }

                int? providerID = null;
                if (user.RoleID == 3)
                    providerID = ProviderDAL.GetProviderIDByUserID(user.UserID);

                SessionManager.Login(user, providerID);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { ShowError($"Login failed: {ex.Message}"); }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text    = "LOGIN";
            }
        }

        private void ShowError(string msg) => lblError.Text = "⚠  " + msg;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { btnLogin_Click(null, EventArgs.Empty); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
