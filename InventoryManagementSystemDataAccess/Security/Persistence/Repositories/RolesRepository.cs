using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityApplicationCore.Models;
using SecurityApplicationDataAccess.Mappers;
using SecurityApplicationLogic.IRepositories;

namespace SecurityApplicationDataAccess.Persistence.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        //Any role operations not exists in RoleManager can be implemented here using the DbContext directly.
    }
}
