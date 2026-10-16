using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IUserService
    {
        public Task<Guid> Create(User user);

        public Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId);

        public Task<User?> Get(Guid userId);

        public Task Update(User user);

        public Task Delete(Guid userId);
    }
}
