using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityApplicationCore.Models;

namespace SecurityApplicationLogic.IRepositories
{
    public interface IUsersRepository
    {
        Task CreateUser(User user);
        Task DeleteUser(Guid userID);
        Task<User?> GetUser(Guid userID);
        Task<List<User>> ListUsers();

        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserWithPasswordByIdAsync(Guid userId);
        Task<List<User>> ListUsersByUsernameOrEmail(string? name, string email);
        Task<User?> FindUserByUsername(string username);

        Task<List<string?>> GetUsernamesStartsWith(string username);

        Task<bool> MarkEmailAsVerified(string tokenEmail);
        Task UpdatePassword(User updatedUser);
        Task UpdateAccountStatus(Guid userId, UserAccountStatus suspended);
        Task<int> UpdateUserRoles(Guid userId, List<Role> roles);
        Task<List<User>> ListUsersByIds(List<Guid> userIds);
    }
}
