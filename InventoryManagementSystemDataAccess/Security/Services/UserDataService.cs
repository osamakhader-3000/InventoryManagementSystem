using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityApplicationDataAccess.Mappers;
using SecurityApplicationDataAccess.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Services
{
    public class UserDataService : SecurityApplicationLogic.Interfaces.IUserDataService
    {
        private readonly UserManager<SecurityApplicationDataAccess.Models.User> _userManager;

        private readonly SecurityApplicationDataAccessContext _context;

        public UserDataService(UserManager<SecurityApplicationDataAccess.Models.User> userManager, SecurityApplicationDataAccessContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Task AddToRoleAsync(Guid userId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task CreateAsync(SecurityApplicationCore.Models.User user)
        {
            var userEntity = user.ToDataAccessModel();
            userEntity.UserRoles = user.Roles.ToUserRoleDataAccessModel().ToList();
            IdentityResult? createResult = null;

            if (string.IsNullOrWhiteSpace(user.Password))
                createResult = await _userManager.CreateAsync(userEntity);
            else
                createResult = await _userManager.CreateAsync(userEntity, user.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }
        }

        public Task<SecurityApplicationCore.Models.User?> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<SecurityApplicationCore.Models.User?> FindByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> GetRolesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<SecurityApplicationCore.Models.User> VerifyPassword(string username, string password)
        {
            SecurityApplicationDataAccess.Models.User? user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.UserName != null && u.UserName == username);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (user.PasswordHash is null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }


            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            SecurityApplicationCore.Models.User userDomainModel = SecurityApplicationCore.Models.User.Reconstitute(
                user.Id,
                user.Email??string.Empty,
                user.UserName,
                user.PasswordHash,
                user.LocalAccount??true,
                user.EmailVerificationStatus == null ? SecurityApplicationCore.Models.UserEmailVerificationStatus.Unverified : (SecurityApplicationCore.Models.UserEmailVerificationStatus)user.EmailVerificationStatus,
                user.AccountStatus == null ? SecurityApplicationCore.Models.UserAccountStatus.Deleted : (SecurityApplicationCore.Models.UserAccountStatus)user.AccountStatus,
                user.UserRoles?.ToCoreModel().ToList() ?? new List<SecurityApplicationCore.Models.Role>()
            );

            return userDomainModel;
        }
    }
}
