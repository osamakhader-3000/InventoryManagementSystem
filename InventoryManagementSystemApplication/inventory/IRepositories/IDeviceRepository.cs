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
        Task<Device?> GetBySerialNumber(string serialNumber);
        Task<List<Device>> List(string? model,string? serialNumber,DateTime? purchasedDate);
        Task Update(Device device);
        Task Delete(Guid id);
        Task<bool> MarkTaken(Guid id);
        Task MarkAvailable(Guid id);
        Task<bool> MarkSold(Guid id, decimal soldPrice);
    }
}
