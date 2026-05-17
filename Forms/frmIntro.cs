using SERVIGO.Helpers;
using SERVIGO.Theme;
using SERVIGO.Forms.Admin;
using SERVIGO.Forms.Customer;
using SERVIGO.Forms.Provider;

namespace SERVIGO.Forms
{
    public partial class frmIntro : Form
    {
        public frmIntro()
        {
            InitializeComponent();
            AddServiceChips();
        }

        private void AddServiceChips()
        {
            foreach (var label in new[] {
                "⚡ Electrical", "🔧 Plumbing", "🚗 Mechanic",
                "🧺 Laundry",   "🎨 Painting", "🪚 Carpenter",
                "🧹 Cleaning",  "❄️ AC Repair" })
            {
                flowChips.Controls.Add(new Label
                {
                    Text      = label,
                    Font      = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextMuted,
                    BackColor = AppTheme.Surface2,
                    AutoSize  = true,
                    Padding   = new Padding(10, 5, 10, 5),
                    Margin    = new Padding(0, 0, 8, 8)
                });
            }
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            using var frm = new frmLogin();
            frm.ShowDialog(this);
            if (SessionManager.IsLoggedIn) OpenDashboard();
        }

        private void btnSignup_Click(object? sender, EventArgs e)
        {
            using var frm = new frmSignup();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show("Account created! Please login.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnLogin_Click(null, EventArgs.Empty);
            }
        }

        private void OpenDashboard()
        {
            Hide();
            Form dash = SessionManager.CurrentUser!.RoleID switch
            {
                1 => new frmAdminDashboard(),
                2 => new frmCustomerDashboard(),
                3 => new frmProviderDashboard(),
                _ => throw new InvalidOperationException()
            };
            dash.FormClosed += (s, a) => { SessionManager.Logout(); Show(); };
            dash.Show();
        }
    }
}
