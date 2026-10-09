using SecurityApplicationCore.Models;
using SecurityApplicationLogic.IRepositories;

namespace SecurityApplicationLogic.Services
{
    public class RolesService : Interfaces.IRoleService
    {
        private readonly Interfaces.IRoleDataService _roleDataService;

        public RolesService(Interfaces.IRoleDataService roleDataService)
        {
            _roleDataService = roleDataService;
        }


        public async Task<Role?> GetRoleByName(string name)
        {
            return await _roleDataService.GetRoleByNameAsync(name);
        }

        public async Task CreateRole(Role role) 
        {
            await _roleDataService.CreateRoleAsync(role);
        }

        public async Task<Role?> GetDefaultRole()
        {
            Role? role = await _roleDataService.GetRoleByNameAsync("employee");
            return role;
        }

    }
}
