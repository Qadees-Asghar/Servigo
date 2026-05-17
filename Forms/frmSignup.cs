using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Models;
using SERVIGO.Theme;

namespace SERVIGO.Forms
{
    public partial class frmSignup : Form
    {
        public frmSignup()
        {
            InitializeComponent();
            SetupPlaceholders();
            LoadCategories();
        }

        private void SetupPlaceholders()
        {
            AppTheme.AddPlaceholder(txtFullName,    "e.g. Ali Hassan");
            AppTheme.AddPlaceholder(txtEmail,       "you@example.com");
            AppTheme.AddPlaceholder(txtPhone,       "03XXXXXXXXX");
            AppTheme.AddPlaceholder(txtCNIC,        "XXXXXXXXXXXXX");
            AppTheme.AddPlaceholder(txtDescription, "Briefly describe your services…");

            txtPhone.KeyPress += DigitsOnly_KeyPress;
            txtCNIC.KeyPress  += DigitsOnly_KeyPress;
        }

        private void rbProvider_CheckedChanged(object? sender, EventArgs e)
        {
            bool show = rbProvider.Checked;
            pnlProviderExt.Visible = show;
            pnlProviderExt.Height  = show ? 182 : 0;
        }

        private void LoadCategories()
        {
            try
            {
                var dt = ProviderDAL.GetCategories();
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember   = "CategoryID";
                cboCategory.DataSource    = dt;
            }
            catch { }
        }

        private void DigitsOnly_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void btnSignup_Click(object? sender, EventArgs e)
        {
            lblError.Text  = string.Empty;
            lblUserID.Text = string.Empty;

            string fullName = AppTheme.GetText(txtFullName, "e.g. Ali Hassan");
            string email    = AppTheme.GetText(txtEmail,    "you@example.com");
            string phone    = AppTheme.GetText(txtPhone,    "03XXXXXXXXX");
            string cnic     = AppTheme.GetText(txtCNIC,     "XXXXXXXXXXXXX");
            string password = txtPassword.Text;
            string confirm  = txtConfirm.Text;

            if (!ValidationHelper.ValidateSignup(fullName, email, phone,
                    cnic, password, confirm, out string error))
            { ShowError(error); return; }

            if (UserDAL.EmailExists(email)) { ShowError("This email is already registered."); return; }
            if (UserDAL.PhoneExists(phone)) { ShowError("This phone number is already registered."); return; }
            if (UserDAL.CNICExists(cnic))   { ShowError("This CNIC is already registered."); return; }

            if (rbProvider.Checked)
            {
                if (cboCategory.SelectedValue == null)
                { ShowError("Please select a service category."); return; }

                string desc = AppTheme.GetText(txtDescription, "Briefly describe your services…");
                if (string.IsNullOrWhiteSpace(desc))
                { ShowError("Please add a description for your services."); return; }
            }

            try
            {
                int    roleID = rbProvider.Checked ? 3 : 2;
                string userID = UserDAL.GenerateUserID();

                User newUser = roleID == 2
                    ? new CustomerUser(userID, fullName, email, phone, cnic)
                    : new ServiceProviderUser(userID, fullName, email, phone, cnic,
                          Convert.ToInt32(cboCategory.SelectedValue),
                          AppTheme.GetText(txtDescription, "Briefly describe your services…"));

                newUser.PasswordHash = PasswordHelper.Hash(password);
                UserDAL.CreateUser(newUser);

                if (roleID == 3)
                {
                    int    catID = Convert.ToInt32(cboCategory.SelectedValue);
                    string desc  = AppTheme.GetText(txtDescription, "Briefly describe your services…");
                    ProviderDAL.CreateProvider(catID, userID, desc);
                }

                lblUserID.Text =
                    $"✔  Account created!  Your User ID:  {userID}\n" +
                    "   Save this ID — you need it every time you login.";

                MessageBox.Show(
                    $"Account created successfully!\n\n" +
                    $"Your User ID:  {userID}\n\n" +
                    "Please save this ID — you will need it every time you login.",
                    "SERVIGO – Account Created",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { ShowError($"Signup failed: {ex.Message}"); }
        }

        private void ShowError(string msg) => lblError.Text = "⚠  " + msg;
    }
}
