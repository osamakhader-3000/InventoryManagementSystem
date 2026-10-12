using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IDeviceRepository
    {
        Task Create(Device device);
        Task<Device?> GetById(Guid id);
        Task<List<Device>> ListAll();
        Task Update(Device device);
        Task Delete(Guid id);
    }
}
