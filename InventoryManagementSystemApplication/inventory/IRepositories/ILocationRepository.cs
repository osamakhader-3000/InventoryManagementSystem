using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface ILocationRepository
    {
        Task Create(Location location);
        Task<Location?> GetById(Guid id);
        Task<List<Location>> ListAll(string? name);
        Task Update(Location location);
        Task Delete(Guid id);
    }
}
