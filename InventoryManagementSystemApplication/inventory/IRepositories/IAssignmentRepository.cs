using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IAssignmentRepository
    {
        Task<long> Create(Assignment assignment);
        Task<Assignment?> GetById(long id);
        Task<List<Assignment>> ListAll(Guid? userId, Guid? depId, DateTime? purDate);
        Task Update(Assignment assignment);
        Task Delete(long id);
    }
}
