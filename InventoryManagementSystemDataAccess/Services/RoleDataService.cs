using InventoryManagementSystemDataAccess.Mappers;
using InventoryManagementSystemDataAccess.Mappers.Security;
using InventoryManagementSystemDataAccess.Models.Security;
using Microsoft.AspNetCore.Identity;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Services
{
    public class RoleDataService : IRoleDataService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly SecurityApplicationLogic.IRepositories.IRolesRepository _rolesRepository;

        public RoleDataService(RoleManager<Role> roleManager, SecurityApplicationLogic.IRepositories.IRolesRepository rolesRepository)
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

            Role? role = await _roleManager.FindByNameAsync(roleName);
           
            return role?.ToCoreModel();
        }
    }
}
