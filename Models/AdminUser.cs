namespace SERVIGO.Models
{
    public class AdminUser : User
    {
        public AdminUser() : base() { RoleID = 1; }

        public AdminUser(string userID, string fullName, string email,
                         string phone, string cnic)
            : base(userID, fullName, email, phone, cnic, 1) { }

        public override string GetRoleName() => "Admin";

        public override void ShowDashboard()
        {
            // Opened by frmIntro after login – handled in Program flow
        }
    }
}
