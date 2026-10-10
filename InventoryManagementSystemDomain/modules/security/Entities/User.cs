
namespace SecurityApplicationCore.Models
{
    public class User : AggregateRoot<Guid>
    {
        public string? Username { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string? Password { get; private set; }
        public UserAccountStatus AccountStatus { get; private set; }
        public UserEmailVerificationStatus EmailVerificationStatus { get; private set; }
        public bool LocalAccount { get; private set; }

        public IEnumerable<Role> Roles
        {
            get
            {
                return _roles;
            }
        }

        private readonly List<Role> _roles = new();

        private User(Guid id) : base(id)
        {
        }

        public static User Create(string email, string? username,string? password, bool localAccount, UserEmailVerificationStatus userEmailVerificationStatus, UserAccountStatus accountStatus)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new Exception("Email is required");

            if (!IsValidEmailFormat(email))
                throw new Exception("Invalid email format");

            if(!IsValidUsernameFormat(username))
                throw new Exception("Username can only contain letters, numbers, underscores and dots, and must be between 5 and 50 characters.");

            if(localAccount && string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required for local accounts.");

            if (!string.IsNullOrWhiteSpace(password) && !IsValidPasswordFormat(password))
                throw new Exception("Password must be between 8 and 50 characters.");

            return new User(Guid.NewGuid())
            {
                Email = email,
                Username = username,
                Password = password,
                LocalAccount = localAccount,
                EmailVerificationStatus = userEmailVerificationStatus,
                AccountStatus = accountStatus
            };
        }

        private static bool IsValidPasswordFormat(string password)
        {           
            if (password.Length < 8 || password.Length > 50)
                return false;
            return true;
        }

        private static bool IsValidUsernameFormat(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
            if (username.Length < 5 || username.Length > 50)
                return false;
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                    return false;
            }
            return true;
        }

        public void UpdatePassword(string newPassword)
        {
            this.Password = newPassword;
        }

        public void AddRole(Role role)
        {
            if (!_roles.Any(r => r.Id == role.Id))
            {
                _roles.Add(role);
            }
        }

        public void RemoveRole(Role role)
        {
            _roles.RemoveAll(r => r.Id == role.Id);
        }

        public bool IsUserMissingRole()
        {
            if (_roles != null && _roles.Count == 1 && _roles.First().Id == Guid.Empty)
            {
                return true;
            }

            if (_roles == null || _roles.Count == 0)
            {
                return true;
            }

            return false;
        }

        private static bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void AddRoles(IEnumerable<Role> roles)
        {
            foreach (Role role in roles)            
                AddRole(role);            
        }

        public static User Reconstitute(
        Guid id, string email, string? userName, string? passwordHash, bool localAccount,
        UserEmailVerificationStatus emailStatus, UserAccountStatus accountStatus,
        IEnumerable<Role> roles)
        {
            var user = new User(id)
            {
                Email = email,
                Username = userName,
                Password = passwordHash,
                LocalAccount = localAccount,
                EmailVerificationStatus = emailStatus,
                AccountStatus = accountStatus
            };
            user._roles.AddRange(roles);
            return user;
        }

    }




    public enum UserAccountStatus
    {   
        Temporary,
        Active,
        Suspended,
        Deleted
    }

    public enum UserEmailVerificationStatus
    {
        Unverified,
        Verified
    }

    public enum UserRole
    {
        Employee = 1,
        TechnicalSupport = 2,
        Administrator = 3
    }

    public enum LoginResult
    {
        Success,
        InvalidCredentials,
        AccountSuspended,
        AccountDeleted,
        EmailNotVerified
    }


}
