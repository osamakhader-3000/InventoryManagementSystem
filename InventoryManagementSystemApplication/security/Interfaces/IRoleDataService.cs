using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IRoleDataService
    {
        Task CreateRoleAsync(Role role);
        Task<SecurityApplicationCore.Models.Role?> GetRoleByNameAsync(string roleName);
    }
}
