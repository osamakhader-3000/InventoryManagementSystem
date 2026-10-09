using Microsoft.Extensions.Options;
using SecurityApplicationCore.Models;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationLogic.IRepositories;
using SecurityApplicationLogic.Settings;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace SecurityApplicationLogic.Services
{
    public class UserService : Interfaces.IUserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IEmailSender _emailSender;
        private readonly Settings.ActivateAccountSettings _activateAccountSettings;
        private readonly Settings.EmailVerificaionSettings _emailVerificationSettings;
        private readonly Settings.JwtSettings _jwtSettings;
        private readonly IUserDataService _userAccountService;
        private readonly IRoleDataService _roleAccountService;

        public UserService(IOptions<ActivateAccountSettings> activateAccountSettings, IOptions<EmailVerificaionSettings> emailVerificationSettings, IOptions<JwtSettings> jwtSettings, IUsersRepository usersRepository, IEmailSender emailSender, IUserDataService userAccountService, IRoleDataService roleAccountService)
        {
            _usersRepository = usersRepository;
            _emailSender = emailSender;
            _activateAccountSettings = activateAccountSettings.Value;
            _emailVerificationSettings = emailVerificationSettings.Value;
            _jwtSettings = jwtSettings.Value;
            _userAccountService = userAccountService;
            _roleAccountService = roleAccountService;
        }

        public async Task<Guid> CreateNewLocalUser(User user)
        {
            await _userAccountService.CreateAsync(user);

            return user.Id;
        }
        public async Task<List<User>> ListUsers()
        {
            List<User> users = await _usersRepository.ListUsers();
            return users;
        }
        public async Task<User?> GetUser(Guid userID)
        {
            User? user = await _usersRepository.GetUser(userID);
            return user;
        }
      

        public async Task<User> CreateNewLocalAccount(string username, string email, string password)
        {
            List<User> users = await _usersRepository.ListUsersByUsernameOrEmail(username, email);

            if (EmailAlreadyExists(users, email))
            {
                throw new ValidationException("Email already in use");
            }

            if (UserNameAlreadyExists(users, username))
            {
                throw new ValidationException("Username already in use");
            }

            User user = User.Create(email, username, password, true, UserEmailVerificationStatus.Unverified, UserAccountStatus.Temporary);

            List<Role> roles = new List<Role>();

            if (user.IsUserMissingRole())
            {
                Role? role = await GetDefaultRole(user);
                if (role != null)                
                    roles.Add(role);
            }
            else
            {
                roles.AddRange(user.Roles);
            }



            Guid id = await CreateNewLocalUser(user);

            return user;

            //return User.Create(user.Email,user.Username,  null, true, user.EmailVerificationStatus, user.AccountStatus);
        }

        

        private async Task<Role?> GetDefaultRole(User user)
        {

            Role? role = await _roleAccountService.GetRoleByNameAsync("employee");
            return role;
            

        }

        private static bool EmailAlreadyExists(IEnumerable<User> users, string email)
        {
            return users.Any(u => u.Email.ToLower().Equals(email.ToLower(), StringComparison.OrdinalIgnoreCase));

        }

        private static bool UserNameAlreadyExists(IEnumerable<User> users, string? name)
        {
            return users.Any(u => name != null && u.Username != null && u.Username.ToLower().Equals(name.ToLower(), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User> GetOrCreateExternalUser(string username, string email)
        {
            List<User> users = (List<User>)await _usersRepository.ListUsersByUsernameOrEmail(username, email);

            if (users.Exists(u => u.Email == email))
            {
                return users.First(u => u.Email == email);
            }

            username = TakeTheFirstPartOfEmail(email);

            /*
            if (string.IsNullOrEmpty(username) || UserNameAlreadyExists(users, username) || !SecurityApplicationCommon.Validators.Username.IsValidFormat(username))
            {
                username = TakeTheFirstPartOfEmail(email);
                if(!SecurityApplicationCommon.Validators.Username.IsValidFormat(username))
                    username = $"user{Guid.NewGuid().ToString().Substring(0, 8)}";
                username = await GenerateNewUsernameBasedOn(username);
            }

            bool isEmailRequiresAdminActivation = CheckEmailRequiresAdminActivation(email);

            UserAccountStatus accountStatus = isEmailRequiresAdminActivation ? UserAccountStatus.Temporary : UserAccountStatus.Active;
            */

            UserEmailVerificationStatus emailVerificationStatus = UserEmailVerificationStatus.Verified;

            Role role = new(Guid.NewGuid(), "Employee");

            User user = User.Create(email, username,  null, false,emailVerificationStatus, UserAccountStatus.Active);

            user.AddRole(role);

            await _usersRepository.CreateUser(user);

            return user;
        }

        private async Task<string> GenerateNewUsernameBasedOn(string username)
        {
            string baseUsername = username.ToLower().Trim();

            List<string?> existingUsernames  = await _usersRepository.GetUsernamesStartsWith(baseUsername);

            if (!existingUsernames.Contains(baseUsername))
                return baseUsername;

            int suffix = 1;
            while (existingUsernames.Contains($"{baseUsername}{suffix}"))
            {
                suffix++;
            }

            return $"{baseUsername}{suffix}";

        }


        private static string TakeTheFirstPartOfEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                throw new ValidationException("Invalid email format");
            return email.Split('@').First();
        }

        private bool CheckEmailRequiresAdminActivation(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                throw new ValidationException("Invalid email format");

            var domain = email.Split('@').Last().ToLowerInvariant();
            return !_activateAccountSettings.ActiveByDefaultDomains.Contains(domain);
        }

        public async Task SendAccountVerificationEmail(User user)
        {
            var expiry = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
            var payload = $"{user.Email}|{expiry}";
            byte[] secretKey = System.Text.Encoding.UTF8.GetBytes(_emailVerificationSettings.EmailVerificationTokenSecretKey);
            var signature = ComputeHmac(payload, secretKey);
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{signature}"));

            // URL-safe encoding
            var urlSafeToken = token.Replace('+', '-').Replace('/', '_').TrimEnd('=');

            string verificationLink = $"{_emailVerificationSettings.VerificationUrl}{urlSafeToken}";

            await _emailSender.SendVerificationEmail(user.Email, verificationLink);

        }

        public async Task<bool> VerifyEmailToken(string token)
        {
            try
            {
                // Restore Base64 padding
                var base64 = token.Replace('-', '+').Replace('_', '/');
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                string decoded;
                try
                {
                    decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                }
                catch
                {
                    throw new Exception("Invalid token format.");
                }

                var parts = decoded.Split('|');
                if (parts.Length != 3)
                    throw new Exception("Malformed token.");

                var tokenEmail = parts[0];
                var expiryStr = parts[1];
                var signature = parts[2];


                if (!long.TryParse(expiryStr, out var expiry) || DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
                    throw new Exception("Token has expired.");

                byte[] secretKey = System.Text.Encoding.UTF8.GetBytes(_emailVerificationSettings.EmailVerificationTokenSecretKey);
                var expectedSignature = ComputeHmac($"{tokenEmail}|{expiryStr}", secretKey);

                if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(signature), Encoding.UTF8.GetBytes(expectedSignature)))
                    throw new Exception("Invalid token signature.");


                bool result = await _usersRepository.MarkEmailAsVerified(tokenEmail);

                if (!result)
                    throw new Exception("User not found or email already verified.");

                return true;

            }
            catch
            {
                return false;
            }
        }


        public async Task<User> GetValidatedUser(string username, string password)
        {
            SecurityApplicationCore.Models.User? user = await _usersRepository.FindUserByUsername(username);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (user.Password is null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            bool verified = PasswordHasher.VerifyPassword(password, user.Password);

            if (!verified)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            return user;
        }

        static string ComputeHmac(string data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash);
        }

        public async Task ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            SecurityApplicationCore.Models.User? user = await _usersRepository.GetUserWithPasswordByIdAsync(userId);

            if (user is null || user.Password is null)
                throw new Exception("User not found or password not set.");


            bool verified = PasswordHasher.VerifyPassword(oldPassword, user.Password);

            if (!verified)
            {
                throw new Exception("Invalid password.");
            }

            user.UpdatePassword(PasswordHasher.HashPassword(newPassword));

            await _usersRepository.UpdatePassword(user);

        }


        public async Task SuspendUser(Guid userId)
        {
            SecurityApplicationCore.Models.User? user = await _usersRepository.GetUserByIdAsync(userId);
            if (user is null)
                throw new Exception("User not found.");
            await _usersRepository.UpdateAccountStatus(userId, UserAccountStatus.Suspended);
        }

        public async Task ActivateUser(Guid userId)
        {
            SecurityApplicationCore.Models.User? user = await _usersRepository.GetUserByIdAsync(userId);
            if (user is null)
                throw new Exception("User not found.");
            await _usersRepository.UpdateAccountStatus(userId, UserAccountStatus.Active);
        }

        public async Task<int> ChangeUserRole(Guid userId, Guid roleId)
        {
            Role role = new Role(roleId, "Employee");
            int result = await _usersRepository.UpdateUserRoles(userId, new List<Role> { role });   
            return result;
        }

        public async Task<List<User>> ListUsersByIds(List<Guid> userIds)
        {
            List<User> users = await _usersRepository.ListUsersByIds(userIds);
            return users;
        }


    }

}
