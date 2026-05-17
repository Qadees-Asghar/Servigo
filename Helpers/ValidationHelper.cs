using System.Text.RegularExpressions;

namespace SERVIGO.Helpers
{
    public static class ValidationHelper
    {
        // ── CNIC ─────────────────────────────────────────────────────────────
        public static bool IsValidCNIC(string value, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(value))        { error = "CNIC is required.";                      return false; }
            if (!Regex.IsMatch(value, @"^\d+$"))         { error = "CNIC must contain digits only.";         return false; }
            if (value.Length != 13)                      { error = "CNIC must be exactly 13 digits.";        return false; }
            return true;
        }

        // ── Phone ─────────────────────────────────────────────────────────────
        public static bool IsValidPhone(string value, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(value))        { error = "Phone number is required.";              return false; }
            if (!Regex.IsMatch(value, @"^\d+$"))         { error = "Phone must contain digits only.";        return false; }
            if (value.Length != 11)                      { error = "Phone must be exactly 11 digits.";       return false; }
            if (!value.StartsWith("0"))                  { error = "Phone must start with 0 (e.g. 03XX…)."; return false; }
            return true;
        }

        // ── Email ─────────────────────────────────────────────────────────────
        public static bool IsValidEmail(string value, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(value)) { error = "Email is required."; return false; }
            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { error = "Enter a valid email address."; return false; }
            return true;
        }

        // ── Password ──────────────────────────────────────────────────────────
        // Rules: at least 1 letter + 1 digit, no specific min length (user's choice)
        public static bool IsValidPassword(string value, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(value))          { error = "Password is required.";                         return false; }
            if (value.Length < 6)                     { error = "Password must be at least 6 characters.";       return false; }
            if (!Regex.IsMatch(value, @"[a-zA-Z]"))   { error = "Password must include at least one letter.";    return false; }
            if (!Regex.IsMatch(value, @"[0-9]"))      { error = "Password must include at least one digit.";     return false; }
            return true;
        }

        // ── Full Name ─────────────────────────────────────────────────────────
        public static bool IsValidFullName(string value, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(value))    { error = "Full name is required.";                   return false; }
            if (value.Trim().Length < 3)             { error = "Full name must be at least 3 characters."; return false; }
            if (!Regex.IsMatch(value, @"^[a-zA-Z\s]+$"))
            { error = "Name can only contain letters and spaces."; return false; }
            return true;
        }

        // ── Composite signup validation ───────────────────────────────────────
        public static bool ValidateSignup(
            string fullName, string email, string phone,
            string cnic, string password, string confirmPassword,
            out string error)
        {
            return IsValidFullName(fullName, out error)
                && IsValidEmail(email, out error)
                && IsValidPhone(phone, out error)
                && IsValidCNIC(cnic, out error)
                && IsValidPassword(password, out error)
                && PasswordsMatch(password, confirmPassword, out error);
        }

        public static bool PasswordsMatch(string password, string confirm, out string error)
        {
            error = string.Empty;
            if (password != confirm) { error = "Passwords do not match."; return false; }
            return true;
        }
    }
}
