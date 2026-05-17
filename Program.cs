using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Models;
using SERVIGO.Forms;

namespace SERVIGO
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            try
            {
                DatabaseHelper.Initialize();

                if (!DatabaseHelper.TestConnection(out string err))
                {
                    MessageBox.Show(
                        $"Cannot connect to SQL Server.\n\n{err}\n\n" +
                        "Please ensure SQL Server is running and the connection string\n" +
                        "in appsettings.json is correct.",
                        "SERVIGO – Connection Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SeedDefaultAdmin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Startup error:\n\n{ex.Message}",
                    "SERVIGO – Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new frmIntro());
        }

        private static void SeedDefaultAdmin()
        {
            if (UserDAL.AdminExists()) return;

            var admin = new AdminUser
            {
                UserID       = "SRV-00001",
                FullName     = "System Administrator",
                Email        = "admin@servigo.com",
                Phone        = "03001234567",
                CNIC         = "1234567890123",
                PasswordHash = PasswordHelper.Hash("Admin@123"),
                RoleID       = 1,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };

            UserDAL.CreateUser(admin);
        }
    }
}
