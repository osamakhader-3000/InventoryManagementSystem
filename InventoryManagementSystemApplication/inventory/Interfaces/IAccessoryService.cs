using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IAccessoryService
    {
        public Task<Guid> Create(Accessory accessory, Guid agentId);

        public Task<List<Accessory>> List(string? name,bool? stock, DateTime? date);

        public Task<Accessory?> Get(Guid accessoryId);

        public Task Update(Accessory accessory, Guid agentId);

        public Task Delete(Guid accessoryId, Guid agentId);
        public Task<IEnumerable<Accessory>> ListByIds(List<Guid> ids);
    }
}
