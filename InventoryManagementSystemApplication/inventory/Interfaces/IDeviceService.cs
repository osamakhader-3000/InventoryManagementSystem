using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IDeviceService
    {
        public Task<Guid> Create(Device device, Guid agentId);

        public Task<List<Device>> List(string? model,string? serialNumber,DateTime? purchasedDate,int? isAssigned);

        public Task<Device?> Get(Guid deviceId);

        public Task<Device?> GetBySerialNumber(string serialNumber);

        public Task Update(Device device, Guid agentId);

        public Task Delete(Guid deviceId, Guid agentId);

        public Task<bool> Sell(Guid deviceId, decimal soldPrice, Guid agentId);
    }
}
