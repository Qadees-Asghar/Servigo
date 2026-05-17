namespace SERVIGO.Models
{
    public class CustomerUser : User
    {
        public CustomerUser() : base() { RoleID = 2; }

        public CustomerUser(string userID, string fullName, string email,
                            string phone, string cnic)
            : base(userID, fullName, email, phone, cnic, 2) { }

        public override string GetRoleName() => "Customer";

        public override void ShowDashboard()
        {
            // Opened by frmIntro after login – handled in Program flow
        }
    }
}
