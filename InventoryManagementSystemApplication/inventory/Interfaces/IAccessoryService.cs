using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IAccessoryService
    {
        public Task<Guid> Create(Accessory accessory);

        public Task<List<Accessory>> List();

        public Task<Accessory?> Get(Guid accessoryId);

        public Task Update(Accessory accessory);

        public Task Delete(Guid accessoryId);
    }
}
