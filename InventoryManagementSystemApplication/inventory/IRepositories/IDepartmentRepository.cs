using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IDepartmentRepository
    {
        Task Create(Department department);
        Task<Department?> GetById(Guid id);
        Task<List<Department>> ListAll(string? name);
        Task Update(Department department);
        Task Delete(Guid id);
    }
}
