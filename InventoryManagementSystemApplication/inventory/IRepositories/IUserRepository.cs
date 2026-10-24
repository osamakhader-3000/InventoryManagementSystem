using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task Create(User user);
        Task<User?> GetById(Guid id);
        Task<User?> GetByName(string name);
        Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId, UserSuspensionState? status);
        Task Update(User user);
        Task<bool> Suspend(Guid id);
        Task<bool> Unsuspend(Guid id);
    }
}
