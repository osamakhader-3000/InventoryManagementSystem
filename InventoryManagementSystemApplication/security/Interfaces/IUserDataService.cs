using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUserDataService
    {
        Task CreateAsync(SecurityApplicationCore.Models.User user);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        Task AddToRoleAsync(Guid userId, string role);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
        Task<SecurityApplicationCore.Models.User?> FindByEmailAsync(string email);
        Task<SecurityApplicationCore.Models.User?> FindByIdAsync(Guid userId);
        Task<SecurityApplicationCore.Models.User> VerifyPassword(string username, string password);
        Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
