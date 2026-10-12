using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IDeviceService
    {
        public Task<Guid> Create(Device device);

        public Task<List<Device>> List();

        public Task<Device?> Get(Guid deviceId);

        public Task Update(Device device);

        public Task Delete(Guid deviceId);
    }
}
