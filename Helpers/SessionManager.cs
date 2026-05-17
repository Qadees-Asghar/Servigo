using SERVIGO.Models;

namespace SERVIGO.Helpers
{
    public static class SessionManager
    {
        public static User?   CurrentUser      { get; private set; }
        public static int?    CurrentProviderID { get; private set; }

        public static bool IsLoggedIn  => CurrentUser != null;
        public static bool IsAdmin     => CurrentUser?.RoleID == 1;
        public static bool IsCustomer  => CurrentUser?.RoleID == 2;
        public static bool IsProvider  => CurrentUser?.RoleID == 3;

        public static void Login(User user, int? providerID = null)
        {
            CurrentUser       = user;
            CurrentProviderID = providerID;
        }

        public static void Logout()
        {
            CurrentUser       = null;
            CurrentProviderID = null;
        }
    }
}
