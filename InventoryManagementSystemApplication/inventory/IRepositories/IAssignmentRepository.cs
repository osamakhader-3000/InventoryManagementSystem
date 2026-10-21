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
        Task<List<Assignment>> ListAll(Guid? userId, Guid? depId, Guid? locationId, DateTime? purDate, string? model, string? serialNumber, bool? isReturned);
        Task<bool> Update(Assignment assignment);
        Task<bool> Delete(long id);
        Task<IEnumerable<Assignment>> ListByAccessoryId(Guid accessoryId);
    }
}
