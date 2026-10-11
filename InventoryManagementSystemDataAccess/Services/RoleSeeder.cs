using InventoryManagementSystemDataAccess.Models.Security;
using Microsoft.AspNetCore.Identity;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Services
{
    public class IdentityRoleSeeder : IRoleSeeder
    {
        private readonly RoleManager<Role> _roleManager;

        public IdentityRoleSeeder(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            foreach (var name in SecurityApplicationCore.Models.Roles.All)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await _roleManager.RoleExistsAsync(name))
                    continue;

                var result = await _roleManager.CreateAsync(new Role { Name = name });

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException(
                        $"Failed to seed role '{name}': {errors}");
                }
            }
        }
    }
}