namespace SERVIGO.Models
{
    public class ServiceProviderUser : User
    {
        private int    _providerID;
        private int    _categoryID;
        private string _categoryName = string.Empty;
        private string _description  = string.Empty;
        private bool   _isApproved;
        private decimal _averageRating;

        public int     ProviderID     { get => _providerID;    set => _providerID    = value; }
        public int     CategoryID     { get => _categoryID;    set => _categoryID    = value; }
        public string  CategoryName   { get => _categoryName;  set => _categoryName  = value; }
        public string  Description    { get => _description;   set => _description   = value; }
        public bool    IsApproved     { get => _isApproved;    set => _isApproved    = value; }
        public decimal AverageRating  { get => _averageRating; set => _averageRating = value; }

        public ServiceProviderUser() : base() { RoleID = 3; }

        public ServiceProviderUser(string userID, string fullName, string email,
                                   string phone, string cnic,
                                   int categoryID, string description)
            : base(userID, fullName, email, phone, cnic, 3)
        {
            _categoryID  = categoryID;
            _description = description;
        }

        public override string GetRoleName() => "Service Provider";

        public override void ShowDashboard()
        {
            // Opened by frmIntro after login – handled in Program flow
        }

        public override string GetDisplayInfo()
            => base.GetDisplayInfo() + $"  |  Category: {_categoryName}  |  Approved: {_isApproved}";
    }
}
