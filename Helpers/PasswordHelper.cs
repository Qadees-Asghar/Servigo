using BCrypt.Net;

namespace SERVIGO.Helpers
{
    public static class PasswordHelper
    {
        private const int WorkFactor = 12;

        public static string Hash(string plainPassword)
            => BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);

        public static bool Verify(string plainPassword, string hash)
            => BCrypt.Net.BCrypt.Verify(plainPassword, hash);
    }
}
