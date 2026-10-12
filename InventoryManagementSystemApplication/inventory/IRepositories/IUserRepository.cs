using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task Create(User user);
        Task<User?> GetById(Guid id);
        Task<List<User>> ListAll();
        Task Update(User user);
        Task Delete(Guid id);
    }
}
