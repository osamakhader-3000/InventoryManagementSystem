using SecurityApplicationCore.Models;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> GetRoleByName(string name);
        Task CreateRole(Role role);

        Task<Role?> GetDefaultRole();
    }
}
