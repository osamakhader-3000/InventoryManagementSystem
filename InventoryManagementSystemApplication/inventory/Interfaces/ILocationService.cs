using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface ILocationService
    {
        public Task<Guid> Create(Location location);

        public Task<List<Location>> List(string? name);

        public Task<Location?> Get(Guid locationId);

        public Task Update(Location location);

        public Task Delete(Guid locationId);
    }
}
