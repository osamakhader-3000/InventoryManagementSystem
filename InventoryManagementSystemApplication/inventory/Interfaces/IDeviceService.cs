using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IDeviceService
    {
        public Task<Guid> Create(Device device);

        public Task<List<Device>> List(string? model,string? serialNumber,string? operatingSystem,DateTime? purchasedDate);

        public Task<Device?> Get(Guid deviceId);

        public Task<Device?> GetBySerialNumber(string serialNumber);

        public Task Update(Device device);

        public Task Delete(Guid deviceId);
    }
}
