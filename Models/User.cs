namespace SERVIGO.Models
{
    public abstract class User
    {
        private string _userID       = string.Empty;
        private string _fullName     = string.Empty;
        private string _email        = string.Empty;
        private string _phone        = string.Empty;
        private string _cnic         = string.Empty;
        private string _passwordHash = string.Empty;
        private int    _roleID;
        private bool   _isActive     = true;
        private DateTime _createdAt  = DateTime.Now;

        public string   UserID       { get => _userID;       set => _userID       = value; }
        public string   FullName     { get => _fullName;     set => _fullName     = value; }
        public string   Email        { get => _email;        set => _email        = value; }
        public string   Phone        { get => _phone;        set => _phone        = value; }
        public string   CNIC         { get => _cnic;         set => _cnic         = value; }
        public string   PasswordHash { get => _passwordHash; set => _passwordHash = value; }
        public int      RoleID       { get => _roleID;       set => _roleID       = value; }
        public bool     IsActive     { get => _isActive;     set => _isActive     = value; }
        public DateTime CreatedAt    { get => _createdAt;    set => _createdAt    = value; }

        protected User() { }

        protected User(string userID, string fullName, string email,
                        string phone, string cnic, int roleID)
        {
            _userID   = userID;
            _fullName = fullName;
            _email    = email;
            _phone    = phone;
            _cnic     = cnic;
            _roleID   = roleID;
        }

        // Polymorphic methods – every role must override these
        public abstract void   ShowDashboard();
        public abstract string GetRoleName();

        public virtual string GetDisplayInfo()
            => $"[{_userID}]  {_fullName}  |  {GetRoleName()}";

        public override string ToString() => GetDisplayInfo();
    }
}
