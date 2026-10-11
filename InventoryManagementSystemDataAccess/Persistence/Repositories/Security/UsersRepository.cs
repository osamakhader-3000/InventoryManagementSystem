using InventoryManagementSystemDataAccess.Mappers.Security;
using InventoryManagementSystemDataAccess.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Security
{
    public class UsersRepository : SecurityApplicationLogic.IRepositories.IUsersRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public UsersRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }


        public async Task CreateUser(SecurityApplicationCore.Models.User user)
        {
            User userData = UsersMapper.ToDataAccessModel(user);
            _context.Users.Add(userData);
            await _context.SaveChangesAsync();
        }
      
        public async Task DeleteUser(Guid userID)
        {
            User? user = await _context.Users.FindAsync(userID);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }


        public async Task<SecurityApplicationCore.Models.User?> FindUserByUsername(string username)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            return user == null ? null : UsersMapper.ToCoreModel(user);
        }

        
        public async Task<List<string?>> GetUsernamesStartsWith(string username)
        {
            List<string?> existingUsernames = await _context.Users
             .Where(u => u.UserName != null && u.UserName.StartsWith(username))
             .Select(u => u.UserName)
             .ToListAsync();

            return existingUsernames?? new List<string?>();
        }

        public async Task<SecurityApplicationCore.Models.User?> GetUser(Guid userID)
        {
            User? user = await _context.Users
                .Where(u => u.Id == userID)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AccountStatus = u.AccountStatus,
                    EmailVerificationStatus = u.EmailVerificationStatus,
                    LocalAccount = u.LocalAccount,
                    UserRoles = u.UserRoles,
                })
                .FirstOrDefaultAsync();
            return user == null ? null : UsersMapper.ToCoreModel(user);
        }


        public async Task<SecurityApplicationCore.Models.User?> GetUserByIdAsync(Guid userId)
        {
            User? user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AccountStatus = u.AccountStatus,
                    EmailVerificationStatus = u.EmailVerificationStatus,
                    LocalAccount = u.LocalAccount,
                    UserRoles = u.UserRoles,
                }).FirstOrDefaultAsync();
                
            return user == null ? null : UsersMapper.ToCoreModel(user);
        }

        public async Task<SecurityApplicationCore.Models.User?> GetUserWithPasswordByIdAsync(Guid userId)
        {
            User? user = await _context.Users.FindAsync(userId);
            return user == null ? null : UsersMapper.ToCoreModel(user);
        }

        public async Task<List<SecurityApplicationCore.Models.User>> ListUsers()
        {
            List<User> users = await _context.Users
            .Select(u => new User
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                AccountStatus = u.AccountStatus,
                EmailVerificationStatus = u.EmailVerificationStatus,
                LocalAccount = u.LocalAccount,
                UserRoles = u.UserRoles,
            }).ToListAsync();
            return users.Select(u => UsersMapper.ToCoreModel(u)).ToList();
        }

        public async Task<List<SecurityApplicationCore.Models.User>> ListUsersByUsernameOrEmail(string? name, string email)
        {
            List<User> users = await _context.Users
                .Where(u => (name != null && u.UserName != null && u.UserName.ToLower() == name.ToLower()) || u.Email != null && u.Email.ToLower() == email.ToLower())
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AccountStatus=u.AccountStatus,
                    EmailVerificationStatus=u.EmailVerificationStatus,
                    LocalAccount=u.LocalAccount,
                    UserRoles=u.UserRoles,
                }).ToListAsync();
                
            return users.Select(u => UsersMapper.ToCoreModel(u)).ToList();
        }


        public async Task<bool> MarkEmailAsVerified(string tokenEmail)
        {
            int rows = await _context.Users.Where(u => u.Email == tokenEmail)
                .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.EmailVerificationStatus, (byte)SecurityApplicationCore.Models.UserEmailVerificationStatus.Verified));

            return rows > 0;
        }

        public async Task UpdatePassword(SecurityApplicationCore.Models.User updatedUser)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if (user != null)
                user.PasswordHash = updatedUser.Password;

            _context.SaveChanges();
        }

        public async Task UpdateAccountStatus(Guid userId, SecurityApplicationCore.Models.UserAccountStatus newStatus)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
                user.AccountStatus = (byte)newStatus;
            _context.SaveChanges();
        }

        public async Task<int> UpdateUserRoles(Guid userId, List<SecurityApplicationCore.Models.Role> roles)
        {
            User? dbUser =  _context.Users.FirstOrDefault(u => u.Id == userId);
            if (dbUser != null)
                dbUser.UserRoles = roles.ToUserRoleDataAccessModel().ToList();
            int result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<List<SecurityApplicationCore.Models.User>> ListUsersByIds(List<Guid> userIds)
        {
            List<User> users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u=>new User { 
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();
            return users.Select(u => UsersMapper.ToCoreModel(u)).ToList();
        }
    }
}
