using Microsoft.AspNetCore.Identity;
using SecurityApplicationDataAccess.Mappers;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Services
{
    public class RoleDataService : IRoleDataService
    {
        private readonly RoleManager<SecurityApplicationDataAccess.Models.Role> _roleManager;
        private readonly SecurityApplicationLogic.IRepositories.IRolesRepository _rolesRepository;

        public RoleDataService(RoleManager<SecurityApplicationDataAccess.Models.Role> roleManager, SecurityApplicationLogic.IRepositories.IRolesRepository rolesRepository)
        {
            _roleManager = roleManager;
            _rolesRepository = rolesRepository;
        }

        public async Task CreateRoleAsync(SecurityApplicationCore.Models.Role role)
        {          
            var roleEntity = role.ToDataAccessModel();
            var result = await _roleManager.CreateAsync(roleEntity);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role creation failed: {errors}");
            }
        }

        public async Task<SecurityApplicationCore.Models.Role?> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or whitespace.", nameof(roleName));

            SecurityApplicationDataAccess.Models.Role? role = await _roleManager.FindByNameAsync(roleName);
           
            return role?.ToCoreModel();
        }
    }
}
