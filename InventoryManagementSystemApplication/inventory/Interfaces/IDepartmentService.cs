using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Guid> Create(Department department);

        public Task<List<Department>> List();

        public Task<Department?> Get(Guid departmentId);

        public Task Update(Department department);

        public Task Delete(Guid departmentId);
    }
}
