using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityApplicationCore.Models;
using SecurityApplicationLogic.IRepositories;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Security
{
    public class RolesRepository : IRolesRepository
    {
        //Any role operations not exists in RoleManager can be implemented here using the DbContext directly.
    }
}
