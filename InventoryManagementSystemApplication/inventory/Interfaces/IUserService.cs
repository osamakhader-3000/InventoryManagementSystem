using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IUserService
    {
        public Task<Guid> Create(User user);

        public Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId, UserSuspensionState? status);

        public Task<User?> Get(Guid userId);

        public Task Update(User user);

        // Also returns every open assignment of the user, freeing their devices and accessories
        public Task Suspend(Guid userId, Guid agentId);

        public Task Unsuspend(Guid userId);
    }
}
