using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IAssignmentService
    {
        public Task<long> Create(Assignment assignment, Guid agentId);

        public Task<List<Assignment>> List(Guid? userId, Guid? departmentId, Guid? locationId, DateTime? purchaseDate, string? model, string? serialNumber, bool? isReturned);

        public Task<Assignment?> Get(long assignmentId);

        public Task Update(Assignment assignment, Guid agentId);

        public Task Relocate(Assignment assignment, Guid agentId);

        public Task Return(long assignmentId, Guid agentId);

        Task<IEnumerable<Assignment>> ListByAccessoryId(Guid accessoryId);
    }
}
