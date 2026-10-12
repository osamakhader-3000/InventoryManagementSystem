using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IAssignmentService
    {
        public Task<long> Create(Assignment assignment);

        public Task<List<Assignment>> List();

        public Task<Assignment?> Get(long assignmentId);

        public Task Update(Assignment assignment);

        public Task Delete(long assignmentId);
    }
}
