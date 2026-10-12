using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IAccessoryRepository
    {
        Task Create(Accessory accessory);
        Task<Accessory?> GetById(Guid id);
        Task<List<Accessory>> ListAll();
        Task Update(Accessory accessory);
        Task Delete(Guid id);
    }
}
