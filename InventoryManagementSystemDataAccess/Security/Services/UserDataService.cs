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
    }
}
